using Application.Abstraction;
using Application.Features.Admin.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminRepository(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return roles.Select(r => new RoleDto(r.Id, r.Name!));
    }

    public async Task CreateRoleAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role name cannot be empty.");

        var exists = await _roleManager.RoleExistsAsync(roleName);
        if (exists)
            throw new InvalidOperationException($"Role '{roleName}' already exists.");

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        if (!result.Succeeded)
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    public async Task DeleteRoleAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
            throw new InvalidOperationException("Role not found.");

        // Optional: prevent deletion of built-in roles like "Admin" or "Customer"
        if (role.Name == "Admin" || role.Name == "Customer")
            throw new InvalidOperationException("Cannot delete built-in roles.");

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    public async Task<IEnumerable<UserWithRolesDto>> GetUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = new List<UserWithRolesDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserWithRolesDto(user.Id, user.Email!, user.UserName!, roles.ToList()));
        }

        return result;
    }

    public async Task AssignRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!await _roleManager.RoleExistsAsync(role))
            throw new InvalidOperationException($"Role '{role}' does not exist.");

        var alreadyInRole = await _userManager.IsInRoleAsync(user, role);
        if (alreadyInRole)
            throw new InvalidOperationException($"User already has role '{role}'.");

        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    public async Task RemoveRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!await _roleManager.RoleExistsAsync(role))
            throw new InvalidOperationException($"Role '{role}' does not exist.");

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        if (!result.Succeeded)
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}