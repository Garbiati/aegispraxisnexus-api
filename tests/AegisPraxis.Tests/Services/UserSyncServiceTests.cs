using System.Security.Claims;
using AegisPraxis.Application.Services;
using AegisPraxis.Domain.Entities;
using AegisPraxis.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace AegisPraxis.Tests.Services;

public class UserSyncServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly UserSyncService _service;

    public UserSyncServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _service = new UserSyncService(_userRepoMock.Object);
    }

    [Fact]
    public async Task Should_Create_User_If_Not_Exists()
    {
        // Arrange
        var claims = new[]
        {
            new Claim("sub", "abc-123"),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim("iss", "http://localhost:8080/realms/clinic-Blue-Star")
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _userRepoMock.Setup(r => r.GetByExternalIdAsync("abc-123", "clinic-Blue-Star"))
            .ReturnsAsync((User?)null);

        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.SyncUserAsync(principal);

        // Assert
        result.Email.Should().Be("test@example.com");
        result.FullName.Should().Be("Test User");
        result.TenantId.Should().Be("clinic-Blue-Star");

        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Should_Update_Existing_User()
    {
        // Arrange
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            ExternalId = "abc-123",
            Email = "old@example.com",
            FullName = "Old Name",
            TenantId = "clinic-Blue-Star"
        };

        var claims = new[]
        {
            new Claim("sub", "abc-123"),
            new Claim(ClaimTypes.Email, "new@example.com"),
            new Claim(ClaimTypes.Name, "New Name"),
            new Claim("iss", "http://localhost:8080/realms/clinic-Blue-Star")
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _userRepoMock.Setup(r => r.GetByExternalIdAsync("abc-123", "clinic-Blue-Star"))
            .ReturnsAsync(existingUser);

        _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.SyncUserAsync(principal);

        // Assert
        result.Email.Should().Be("new@example.com");
        result.FullName.Should().Be("New Name");

        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
