using static Application.Types.AuthDTOs;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request, string ipAddress);
        Task<AuthResponse> RegisterAsync(RegisterRequest request, string ipAddress);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress);
        Task RevokeTokenAsync(RevokeTokenRequest request, string ipAddress);
        Task<UserDto?> GetUserByIdAsync(Guid userId);
        Task<bool> ValidateTokenAsync(string token);
    }
}
