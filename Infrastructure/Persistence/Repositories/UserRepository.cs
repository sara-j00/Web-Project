using System.Security.Claims;
using Application.Abstractions;
using Application.Common.Models;
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
        => await _userManager.FindByEmailAsync(email) != null;

    public async Task<bool> UsernameExistsAsync(string username)
        => await _userManager.FindByNameAsync(username) != null;

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

        return user.Id;
    }

    public async Task<UserModel?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user == null ? null : MapToUserModel(user);
    }

    public async Task<UserModel?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user == null ? null : MapToUserModel(user);
    }

    public async Task<UserModel?> GetUserByUsernameAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user == null ? null : MapToUserModel(user);
    }

    public async Task<bool> CheckPasswordAsync(string userId, string password)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IList<string>> GetRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return new List<string>();
        return await _userManager.GetRolesAsync(user);
    }

    public async Task AddToRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception("User not found");
        await _userManager.AddToRoleAsync(user, role);
    }

    private static UserModel MapToUserModel(IdentityUser user)
    {
        return new UserModel
        {
            Id = user.Id,
            Email = user.Email!,
            UserName = user.UserName!
        };
    }
}
