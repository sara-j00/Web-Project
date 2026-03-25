using Application.Abstraction;
using Application.Features.Admin.Dtos;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admin.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(IAdminRepository adminRepository,
        ILogger<AdminService> logger)
    {
        _adminRepository = adminRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        => await _adminRepository.GetAllRolesAsync();

    public async Task CreateRoleAsync(string roleName)
        => await _adminRepository.CreateRoleAsync(roleName);

    public async Task DeleteRoleAsync(string roleId)
        => await _adminRepository.DeleteRoleAsync(roleId);

    public async Task<IEnumerable<UserWithRolesDto>> GetUsersAsync()
        => await _adminRepository.GetUsersAsync();

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
    }
}