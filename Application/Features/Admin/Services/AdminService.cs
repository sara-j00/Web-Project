using Application.Abstraction;
using Application.Features.Admin.Dtos;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admin.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(IAdminRepository adminRepository, ILogger<AdminService> logger)
    {
        _adminRepository = adminRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        _logger.LogInformation("Fetching all roles");
        var roles = await _adminRepository.GetAllRolesAsync();
        _logger.LogInformation("Fetched {Count} roles", roles.Count());
        return roles;
    }

    public async Task CreateRoleAsync(string roleName)
    {
        _logger.LogInformation("Creating role {RoleName}", roleName);
        await _adminRepository.CreateRoleAsync(roleName);
        _logger.LogInformation("Role {RoleName} created", roleName);
    }

    public async Task DeleteRoleAsync(string roleId)
    {
        _logger.LogInformation("Deleting role {RoleId}", roleId);
        await _adminRepository.DeleteRoleAsync(roleId);
        _logger.LogInformation("Role {RoleId} deleted", roleId);
    }

    public async Task<IEnumerable<UserWithRolesDto>> GetUsersAsync()
    {
        _logger.LogInformation("Fetching all users with roles");
        var users = await _adminRepository.GetUsersAsync();
        _logger.LogInformation("Fetched {Count} users", users.Count());
        return users;
    }

    public async Task AssignRoleAsync(string userId, string role)
    {
        _logger.LogInformation("Assigning role {Role} to user {UserId}", role, userId);
        await _adminRepository.AssignRoleAsync(userId, role);
        _logger.LogInformation("Role {Role} assigned to user {UserId}", role, userId);
    }

    public async Task RemoveRoleAsync(string userId, string role)
    {
        _logger.LogInformation("Removing role {Role} from user {UserId}", role, userId);
        await _adminRepository.RemoveRoleAsync(userId, role);
        _logger.LogInformation("Role {Role} removed from user {UserId}", role, userId);
    }
}