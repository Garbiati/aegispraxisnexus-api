using System.Security.Claims;
using AegisPraxis.Domain.Entities;

namespace AegisPraxis.Application.Interfaces;

public interface IUserProfileService
{
    Task<User?> GetCurrentUserAsync(ClaimsPrincipal user);
}
