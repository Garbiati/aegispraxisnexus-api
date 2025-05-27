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

    public UsersController(IUserSyncService syncService, IUserQueryService queryService)
    {
        _syncService = syncService;
        _queryService = queryService;
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
