using System.Security.Claims;
using System.Text.RegularExpressions;
using AegisPraxis.Application.Common;
using AegisPraxis.Application.Interfaces;
using AegisPraxis.Domain.Entities;
using AegisPraxis.Domain.Interfaces;

namespace AegisPraxis.Application.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserRepository _users;

    public UserProfileService(IUserRepository users)
    {
        _users = users;
    }

    public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal user)
    {
        var externalId = user.FindFirst("sub")?.Value;
        var issuer = user.FindFirst("iss")?.Value;

        if (string.IsNullOrEmpty(externalId) || string.IsNullOrEmpty(issuer))
            return null;

        var realm = SecurityHelpers.ExtractRealmFromIssuer(issuer);
        return await _users.GetByExternalIdAsync(externalId, realm);
    }
}
