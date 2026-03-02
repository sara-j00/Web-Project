using System.Security.Claims;

namespace Application.Abstractions;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);

    /// <summary>
    /// Creates a user and returns the generated UserId
    /// </summary>
    Task<string> CreateAsync(string email, string username, string password);
    Task DeleteAsync(ClaimsPrincipal userClaims);
    Task<string?> GetUserIdByEmailAsync(string email);
    Task<string?> GetUserIdByUsernameAsync(string username);
    Task<bool> CheckPasswordAsync(string userId, string password);
    Task SignInAsync(string userId);
    Task SignOutAsync();
}
