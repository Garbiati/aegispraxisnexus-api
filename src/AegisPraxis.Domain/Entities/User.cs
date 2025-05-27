namespace AegisPraxis.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string ExternalId { get; set; } = default!; // Keycloak ID
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string? ActiveRole { get; set; }
    public bool IsActive { get; set; } = true;
    public string TenantId { get; set; } = default!;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
