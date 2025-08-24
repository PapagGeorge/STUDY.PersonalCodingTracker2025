using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<UserController> _logger;

    public UserController(IAuthService authService, ILogger<UserController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var ipAddress = GetIpAddress();
        try
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User not found. UserId={UserId}, IP={IpAddress}", id, ipAddress);
                return NotFound();
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user. UserId={UserId}, IP={IpAddress}", id, ipAddress);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var ipAddress = GetIpAddress();
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Unauthorized access attempt to profile. IP={IpAddress}", ipAddress);
                return Unauthorized();
            }

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Profile not found for UserId={UserId}, IP={IpAddress}", userId, ipAddress);
                return NotFound();
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving profile. IP={IpAddress}", ipAddress);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    private string GetIpAddress()
    {
        var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ipAddress))
            ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

        return ipAddress ?? "Unknown";
    }
}