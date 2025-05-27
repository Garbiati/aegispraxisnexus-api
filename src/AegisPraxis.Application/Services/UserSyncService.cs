using System.Security.Claims;
using System.Text.RegularExpressions;
using AegisPraxis.Domain.Entities;
using AegisPraxis.Domain.Interfaces;
using AegisPraxis.Application.Interfaces;

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
        var externalId = user.FindFirst("sub")?.Value ?? throw new UnauthorizedAccessException("Missing 'sub' claim.");
        var email = user.FindFirst(ClaimTypes.Email)?.Value ?? "unknown@email.com";
        var name = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

        var issuer = user.FindFirst("iss")?.Value ?? throw new UnauthorizedAccessException("Missing 'iss' claim.");
        var realm = ExtractRealmFromIssuer(issuer);

        var existing = await _users.GetByExternalIdAsync(externalId, realm);

        if (existing is null)
        {
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                ExternalId = externalId,
                Email = email,
                FullName = name,
                TenantId = realm,
                IsActive = true
            };

            await _users.AddAsync(newUser);
            await _users.SaveChangesAsync();
            return newUser;
        }

        existing.FullName = name;
        existing.Email = email;

        await _users.SaveChangesAsync();
        return existing;
    }

    private static string ExtractRealmFromIssuer(string issuer)
    {
        var match = Regex.Match(issuer, @"realms\/([^\/]+)");
        return match.Success ? match.Groups[1].Value : throw new InvalidOperationException("Cannot extract realm from issuer.");
    }
}
