using Application.Abstraction;
using Application.Features.Admin.Dtos;

namespace Application.Features.Admin.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;

    public AdminService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
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
        => await _adminRepository.AssignRoleAsync(userId, role);

    public async Task RemoveRoleAsync(string userId, string role)
        => await _adminRepository.RemoveRoleAsync(userId, role);
}