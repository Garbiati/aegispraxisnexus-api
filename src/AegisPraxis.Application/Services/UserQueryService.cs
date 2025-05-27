using System.Security.Claims;
using System.Text.RegularExpressions;
using AegisPraxis.Application.Interfaces;
using AegisPraxis.Domain.Entities;
using AegisPraxis.Domain.Interfaces;

namespace AegisPraxis.Application.Services;

public class UserQueryService : IUserQueryService
{
    private readonly IUserRepository _userRepository;

    public UserQueryService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetUsersByRealmAsync(ClaimsPrincipal user)
    {
        var issuer = user.FindFirst("iss")?.Value
            ?? throw new UnauthorizedAccessException("Missing 'iss' claim.");

        var realm = ExtractRealmFromIssuer(issuer);

        return await _userRepository.GetAllByTenantAsync(realm);
    }

    private static string ExtractRealmFromIssuer(string issuer)
    {
        var match = Regex.Match(issuer, @"realms\/([^\/]+)");
        return match.Success ? match.Groups[1].Value : throw new InvalidOperationException("Cannot extract realm from issuer.");
    }
}
