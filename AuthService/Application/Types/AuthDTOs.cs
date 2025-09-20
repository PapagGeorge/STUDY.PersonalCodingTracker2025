namespace Application.Types
{
    public class AuthDTOs
    {
        public record LoginRequest(string Email, string Password);
        
        public record RegisterRequest(
            string UserName,
            string Email,
            string Password,
            string FirstName,
            string LastName);

        public record AuthResponse(
            Guid UserId,
            string Username,
            string Email,
            string AccessToken,
            string RawRefreshToken,
            DateTime ExpiresAt,
            List<string> Roles);

        public record RefreshTokenRequest(string RefreshToken);

        public record RevokeTokenRequest(string RefreshToken);

        public record UserDto(
            Guid Id,
            string UserName,
            string Email,
            string FirstName,
            string LastName,
            bool IsActive,
            List<string> Roles);

        public record ValidateTokenRequest(string Token);
    }
}