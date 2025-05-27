using System.Security.Claims;
using System.Text.RegularExpressions;
using AegisPraxis.Application.Common;
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

        var realm = SecurityHelpers.ExtractRealmFromIssuer(issuer);

        return await _userRepository.GetAllByTenantAsync(realm);
    }
}
