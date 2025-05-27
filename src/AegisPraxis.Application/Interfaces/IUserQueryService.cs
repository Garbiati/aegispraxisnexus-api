using AegisPraxis.Domain.Entities;
using System.Security.Claims;

namespace AegisPraxis.Application.Interfaces;

public interface IUserQueryService
{
    Task<IEnumerable<User>> GetUsersByRealmAsync(ClaimsPrincipal user);
}
