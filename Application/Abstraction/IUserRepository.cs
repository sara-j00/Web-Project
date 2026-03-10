using Application.Common.Models;

namespace Application.Abstractions;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task<string> CreateAsync(string email, string username, string password);
    Task<UserModel?> GetUserByIdAsync(string userId);
    Task<UserModel?> GetUserByEmailAsync(string email);
    Task<UserModel?> GetUserByUsernameAsync(string username);
    Task<bool> CheckPasswordAsync(string userId, string password);
    Task<IList<string>> GetRolesAsync(string userId);
    Task AddToRoleAsync(string userId, string role);
}
