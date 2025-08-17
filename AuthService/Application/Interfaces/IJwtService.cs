using Domain;
using System.Security.Claims;

namespace Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user, List<string> roles);
        RefreshToken GenerateRefreshToken(string ipAddress);
        ClaimsPrincipal? GetPrincipalFromToken(string token);
        bool ValidateToken(string token);
    }
}
