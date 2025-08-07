using Application.Interfaces;
using Application.Types;

namespace Application.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<AuthDTOs.UserDto?> GetUserByIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthDTOs.AuthResponse> LoginAsync(AuthDTOs.LoginRequest request, string ipAddress)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthDTOs.AuthResponse> RefreshTokenAsync(AuthDTOs.RefreshTokenRequest request, string ipAddress)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthDTOs.AuthResponse> RegisterAsync(AuthDTOs.RegisterRequest request, string ipAddress)
        {
            throw new NotImplementedException();
        }

        public async Task RevokeTokenAsync(AuthDTOs.RevokeTokenRequest request, string ipAddress)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            throw new NotImplementedException();
        }
    }
}
