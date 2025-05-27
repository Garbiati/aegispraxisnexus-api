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


    public UsersController(IUserSyncService syncService)
    {
        _syncService = syncService;
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
}
