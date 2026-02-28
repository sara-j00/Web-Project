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

}
