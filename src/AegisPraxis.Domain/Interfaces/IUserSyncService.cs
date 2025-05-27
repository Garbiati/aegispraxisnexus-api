using System.Security.Claims;
using AegisPraxis.Domain.Entities;

namespace AegisPraxis.Application.Interfaces;

public interface IUserSyncService
{
    Task<User> SyncUserAsync(ClaimsPrincipal user);
}
