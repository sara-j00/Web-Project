using System.Security.Claims;
using Application.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<IdentityUser> _userManager;
    public UserRepository(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }
    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _userManager.FindByNameAsync(username) != null;
    }

    /// <summary>
    /// Creates a user and returns the generated UserId
    /// </summary>
    public async Task<string> CreateAsync(string email, string username, string password)
    {
        var user = new IdentityUser
        {
            Email = email,
            UserName = username,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

        return user.Id; // Returns the Identity user with the Id
    }
    public async Task DeleteAsync(ClaimsPrincipal userClaims)
    {

    }

}
