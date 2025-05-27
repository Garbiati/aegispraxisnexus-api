using AegisPraxis.Application.Interfaces;
using AegisPraxis.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AegisPraxis.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserSyncService _syncService;
    private readonly IUserQueryService _queryService;
    private readonly IUserProfileService _profileService;

    public UsersController(
        IUserSyncService syncService,
        IUserQueryService queryService,
        IUserProfileService profileService)
    {
        _syncService = syncService;
        _queryService = queryService;
        _profileService = profileService;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var user = await _profileService.GetCurrentUserAsync(User);
        if (user is null)
            return NotFound("User not found in the system");

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.TenantId
        });
    }

    [Authorize]
    [HttpPost("me/sync")]
    public async Task<IActionResult> SyncMe()
    {
        var user = await _syncService.SyncUserAsync(User);
        return Ok(new
        {
            message = "User synced successfully",
            user.Id,
            user.Email,
            user.FullName
        });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _queryService.GetUsersByRealmAsync(User);
        return Ok(users.Select(u => new
        {
            u.Id,
            u.Email,
            u.FullName,
            u.TenantId
        }));
    }
}
