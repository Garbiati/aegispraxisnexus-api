using System.Security.Claims;
using System.Text.RegularExpressions;
using AegisPraxis.Domain.Entities;
using AegisPraxis.Domain.Interfaces;
using AegisPraxis.Application.Interfaces;
using AegisPraxis.Application.Common;
using System.Text.Json;

namespace AegisPraxis.Application.Services;

public class UserSyncService : IUserSyncService
{
    private readonly IUserRepository _users;

    public UserSyncService(IUserRepository users)
    {
        _users = users;
    }

    public async Task<User> SyncUserAsync(ClaimsPrincipal user)
    {
        Console.WriteLine("IsAuthenticated: " + user.Identity?.IsAuthenticated);
        foreach (var claim in user.Claims)
            Console.WriteLine($"Claim: {claim.Type} => {claim.Value}");

        var externalId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Missing 'nameidentifier' claim.");

        var email = user.FindFirst(ClaimTypes.Email)?.Value ?? "unknown@email.com";
        var name = user.FindFirst("name")?.Value
                 ?? user.FindFirst(ClaimTypes.Name)?.Value
                 ?? $"{user.FindFirst("given_name")?.Value} {user.FindFirst("family_name")?.Value}"
                 ?? "Unknown";

        var issuer = user.FindFirst("iss")?.Value ?? throw new UnauthorizedAccessException("Missing 'iss' claim.");
        var tenantId = SecurityHelpers.ExtractRealmFromIssuer(issuer);

        var userEntity = await _users.GetByExternalIdAsync(externalId, tenantId);

        if (userEntity == null)
        {
            userEntity = new User
            {
                Id = Guid.NewGuid(),
                ExternalId = externalId,
                Email = email,
                FullName = name,
                TenantId = tenantId,
                IsActive = true
            };
            await _users.AddAsync(userEntity);
        }
        else
        {
            userEntity.Email = email;
            userEntity.FullName = name;
        }

        // PEGAR AS ROLES DO TOKEN
        var roleNames = new List<string>();
        var accessClaim = user.Claims.FirstOrDefault(c => c.Type == "resource_access");

        if (accessClaim is not null)
        {
            using var json = JsonDocument.Parse($"{{ \"resource_access\": {accessClaim.Value} }}");

            if (json.RootElement.TryGetProperty("resource_access", out var resAccess)
                && resAccess.TryGetProperty("account", out var account)
                && account.TryGetProperty("roles", out var rolesElem)
                && rolesElem.ValueKind == JsonValueKind.Array)
            {
                roleNames = rolesElem.EnumerateArray()
                    .Select(r => r.GetString()!)
                    .Distinct()
                    .ToList();
            }
        }

        // Buscar roles existentes no banco (por Tenant)
        var allRoles = await _users.GetRolesByNamesAsync(roleNames, tenantId);

        // Criar roles ausentes
        foreach (var roleName in roleNames.Except(allRoles.Select(r => r.Name)))
        {
            var newRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                TenantId = tenantId
            };
            allRoles.Add(newRole);
            await _users.AddRoleAsync(newRole);
        }

        // Atualizar as roles do usuário (UserRoles)
        var currentRoleIds = userEntity.UserRoles.Select(ur => ur.RoleId).ToList();
        var desiredRoleIds = allRoles.Select(r => r.Id).ToList();

        // Remover UserRoles que não estão mais
        var rolesToRemove = userEntity.UserRoles
            .Where(ur => !desiredRoleIds.Contains(ur.RoleId))
            .ToList();

        foreach (var role in rolesToRemove)
        {
            userEntity.UserRoles.Remove(role);
        }

        // Adicionar UserRoles novas
        foreach (var role in allRoles)
        {
            if (!userEntity.UserRoles.Any(ur => ur.RoleId == role.Id))
            {
                userEntity.UserRoles.Add(new UserRole
                {
                    RoleId = role.Id,
                    UserId = userEntity.Id
                });
            }
        }

        await _users.SaveChangesAsync();
        return userEntity;
    }

}
