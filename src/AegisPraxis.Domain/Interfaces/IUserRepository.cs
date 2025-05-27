using AegisPraxis.Domain.Entities;

namespace AegisPraxis.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllByTenantAsync(string tenantId);
    Task<User?> GetByExternalIdAsync(string externalId, string tenantId);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
