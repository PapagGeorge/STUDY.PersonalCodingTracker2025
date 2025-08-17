using Domain;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<List<string>> GetUserRolesAsync(Guid userId);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task AddUserRoleAsync(Guid userId, Guid roleId);
        Task RevokeUserRoleAsync(Guid userId, Guid roleId);
    }
}