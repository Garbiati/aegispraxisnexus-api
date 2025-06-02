using AegisPraxis.Domain.Entities;
using AegisPraxis.Domain.Interfaces;
using AegisPraxis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AegisPraxis.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AegisDbContext _context;

    public UserRepository(AegisDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllByTenantAsync(string tenantId)
    {
        return await _context.Users
            .Where(u => u.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task<User?> GetByExternalIdAsync(string externalId, string tenantId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.ExternalId == externalId && u.TenantId == tenantId);
    }

    public async Task<List<Role>> GetRolesByNamesAsync(IEnumerable<string> roleNames, string tenantId)
    {
        return await _context.Roles
            .Where(r => roleNames.Contains(r.Name) && r.TenantId == tenantId)
            .ToListAsync();
    }
    public async Task AddRoleAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
