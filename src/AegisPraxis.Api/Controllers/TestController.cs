using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AegisPraxis.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult Public()
    {
        return Ok("✅ Public endpoint: no authentication required.");
    }

    [HttpGet("authenticated")]
    [Authorize]
    public IActionResult Authenticated()
    {
        return Ok($"✅ Authenticated endpoint: welcome, {User.Identity?.Name ?? "unknown"}");
    }

    [HttpGet("admin")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult AdminOnly()
    {
        return Ok("✅ Authorized as admin.");
    }

    [HttpGet("doctor")]
    [Authorize(Policy = "DoctorOnly")]
    public IActionResult DoctorOnly()
    {
        return Ok("✅ Authorized as doctor.");
    }
}
