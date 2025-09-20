using Application.Interfaces;
using Application.Types;
using Domain;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Application.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtService jwtService,
            IPasswordService passwordService,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
            _passwordService = passwordService;
            _logger = logger;
        }
        public async Task<AuthDTOs.UserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userRepository.GetUserRolesAsync(userId);

            return new AuthDTOs.UserDto(
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.IsActive,
                roles
                );
        }

        public async Task<AuthDTOs.AuthResponse> LoginAsync(AuthDTOs.LoginRequest request, string ipAddress)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Failed login attempt. Email={Email}, IP={IpAddress}", request.Email, ipAddress);
                throw new UnauthorizedAccessException("Invalid Credentials");
            }

            if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt. Invalid password. UserId={UserId}, Email={Email}, IP={IpAddress}",
                    user.Id, request.Email, ipAddress);
                throw new UnauthorizedAccessException("Invalid Credentials");
            }

            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            var (rawRefreshToken, refreshToken) = _jwtService.GenerateRefreshToken(ipAddress);
            refreshToken.UserId = user.Id;

            await _refreshTokenRepository.AddAsync(refreshToken);

            _logger.LogInformation("User logged in successfully. UserId={UserId}, Email={Email}, IP={IpAddress}", user.Id, user.Email, ipAddress);

            return new AuthDTOs.AuthResponse(
                user.Id,
                user.UserName,
                user.Email,
                accessToken,
                rawRefreshToken,
                refreshToken.ExpiresAt,
                roles
                );
        }

        public async Task<AuthDTOs.AuthResponse> RefreshTokenAsync(AuthDTOs.RefreshTokenRequest request, string ipAddress)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (refreshToken == null || !refreshToken.IsActive)
            {
                _logger.LogWarning("Invalid refresh token attempt. IP={IpAddress}", ipAddress);
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Refresh token for inactive/nonexistent user. UserId={UserId}, IP={IpAddress}", refreshToken.UserId, ipAddress);
                throw new UnauthorizedAccessException("User not found or inactive");
            }

            //Revoke old token
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;

            //Generate new tokens
            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            var (rawRefreshToken, refreshTokenEntity) = _jwtService.GenerateRefreshToken(ipAddress);
            refreshTokenEntity.UserId = user.Id;

            // Link old to new
            refreshToken.ReplacedByToken = refreshTokenEntity.Token;

            await _refreshTokenRepository.UpdateAsync(refreshToken);
            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            _logger.LogInformation("Refresh token rotated. UserId={UserId}, OldTokenId={OldTokenId}, NewTokenId={NewTokenId}, IP={IpAddress}",
            user.Id, refreshToken.Id, refreshTokenEntity.Id, ipAddress);

            return new AuthDTOs.AuthResponse(
                user.Id,
                user.UserName,
                user.Email,
                accessToken,
                rawRefreshToken,
                refreshToken.ExpiresAt,
                roles
                );
        }

        public async Task<AuthDTOs.AuthResponse> RegisterAsync(AuthDTOs.RegisterRequest request, string ipAddress)
        {
            if (await _userRepository.GetByEmailAsync(request.Email) != null)
            {
                _logger.LogWarning("Registration failed. Email already exists. Email={Email}, IP={IpAddress}", request.Email, ipAddress);
                throw new InvalidOperationException("User with this Email already exists");
            }

            if (await _userRepository.GetByUsernameAsync(request.UserName) != null)
            {
                _logger.LogWarning("Registration failed. Username taken. Username={Username}, IP={IpAddress}", request.UserName, ipAddress);
                throw new InvalidOperationException("Username already taken");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = _passwordService.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            // Assign default role
            var defaultRole = await _roleRepository.GetByNameAsync("User");
            if (defaultRole != null)
            {
                await _userRepository.AddUserRoleAsync(user.Id, defaultRole.Id);
            }

            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            var accessToken = _jwtService.GenerateAccessToken(user, roles);

            // ✅ Generate refresh token (raw + entity)
            var (rawRefreshToken, refreshTokenEntity) = _jwtService.GenerateRefreshToken(ipAddress);
            refreshTokenEntity.UserId = user.Id;

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            _logger.LogInformation("New user registered. UserId={UserId}, Username={Username}, Email={Email}, IP={IpAddress}",
                user.Id, user.UserName, user.Email, ipAddress);

            return new AuthDTOs.AuthResponse(
            user.Id,
            user.UserName,
            user.Email,
            accessToken,
            rawRefreshToken,
            refreshTokenEntity.ExpiresAt,
            roles
            );
        }

        public async Task RevokeTokenAsync(AuthDTOs.RevokeTokenRequest request, string ipAddress)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (refreshToken == null || !refreshToken.IsActive)
            {
                _logger.LogWarning(
                "Attempt to revoke invalid or already revoked token. UserId={UserId}, TokenId={TokenId}, IP={IpAddress}",
                refreshToken?.UserId,
                refreshToken?.Id,
                ipAddress);

                throw new InvalidOperationException("Token not found or already revoked");
            }

            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;

            await _refreshTokenRepository.UpdateAsync(refreshToken);

            _logger.LogInformation("Refresh token revoked. UserId={UserId}, TokenId={TokenId}, IP={IpAddress}",
            refreshToken.UserId, refreshToken.Id, ipAddress);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            return _jwtService.ValidateToken(token);
        }
    }
}
