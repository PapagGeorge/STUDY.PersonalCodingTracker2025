using Application;
using Application.Interfaces;
using Application.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthDTOs.LoginRequest request)
    {
        try
        {
            var ipAddress = GetIpAddress();
            var response = await _authService.LoginAsync(request, ipAddress);

            SetRefreshTokenCookie(response.RawRefreshToken);

            return Ok(new
            {
                userId = response.UserId,
                username = response.Username,
                email = response.Email,
                accessToken = response.AccessToken,
                roles = response.Roles
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Login failed for email {Email} from IP {IpAddress}", request.Email, GetIpAddress());
            return Unauthorized(new { message = "Invalid credentials" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for email {Email} from IP {IpAddress}", request.Email, GetIpAddress());
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthDTOs.RegisterRequest request)
    {
        try
        {
            var ipAddress = GetIpAddress();
            var response = await _authService.RegisterAsync(request, ipAddress);

            SetRefreshTokenCookie(response.RawRefreshToken);

            return Ok(new
            {
                userId = response.UserId,
                username = response.Username,
                email = response.Email,
                accessToken = response.AccessToken,
                roles = response.Roles
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed for email {Email} from IP {IpAddress}", request.Email, GetIpAddress());
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for email {Email} from IP {IpAddress}", request.Email, GetIpAddress());
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            var refreshToken = GetRefreshTokenFromCookie() ?? Request.Headers["RefreshToken"].FirstOrDefault();

            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token request missing from IP {IpAddress}", GetIpAddress());
                return BadRequest(new { message = "Refresh token is required" });
            }

            var ipAddress = GetIpAddress();
            var request = new AuthDTOs.RefreshTokenRequest(refreshToken);
            var response = await _authService.RefreshTokenAsync(request, ipAddress);

            SetRefreshTokenCookie(response.RawRefreshToken);

            return Ok(new
            {
                userId = response.UserId,
                username = response.Username,
                email = response.Email,
                accessToken = response.AccessToken,
                roles = response.Roles
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Refresh token validation failed from IP {IpAddress}", GetIpAddress());
            return Unauthorized(new { message = "Invalid refresh token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during refresh token from IP {IpAddress}", GetIpAddress());
            return StatusCode(500, new { message = "An error occurred during token refresh" });
        }
    }

    [Authorize]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] AuthDTOs.RevokeTokenRequest? request = null)
    {
        try
        {
            var token = request?.RefreshToken ?? GetRefreshTokenFromCookie();

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Revoke token request missing from IP {IpAddress}", GetIpAddress());
                return BadRequest(new { message = "Token is required" });
            }

            var ipAddress = GetIpAddress();
            var revokeRequest = new AuthDTOs.RevokeTokenRequest(token);
            await _authService.RevokeTokenAsync(revokeRequest, ipAddress);

            return Ok(new { message = "Token revoked" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Token revocation failed from IP {IpAddress}", GetIpAddress());
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during token revocation from IP {IpAddress}", GetIpAddress());
            return StatusCode(500, new { message = "An error occurred during token revocation" });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching current user from IP {IpAddress}", GetIpAddress());
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpPost("validate-token")]
    public async Task<IActionResult> ValidateToken([FromBody] AuthDTOs.ValidateTokenRequest request)
    {
        try
        {
            var isValid = await _authService.ValidateTokenAsync(request.Token);
            return Ok(new { isValid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during token validation from IP {IpAddress}", GetIpAddress());
            return StatusCode(500, new { message = "An error occurred during token validation" });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var refreshToken = GetRefreshTokenFromCookie();
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var ipAddress = GetIpAddress();
                var revokeRequest = new AuthDTOs.RevokeTokenRequest(refreshToken);
                await _authService.RevokeTokenAsync(revokeRequest, ipAddress);
            }

            // Clear refresh token cookie
            Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during logout from IP {IpAddress}", GetIpAddress());
            return StatusCode(500, new { message = "An error occurred during logout" });
        }
    }

    private string GetIpAddress()
    {
        var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ipAddress))
            ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

        return ipAddress ?? "Unknown";
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    private string? GetRefreshTokenFromCookie()
    {
        return Request.Cookies["refreshToken"];
    }
}