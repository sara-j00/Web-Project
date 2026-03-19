using Application.Features.Admin.Dtos;

namespace Application.Features.Admin.Services;

public interface IAdminService
{
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task CreateRoleAsync(string roleName);
    Task DeleteRoleAsync(string roleId);
    Task<IEnumerable<UserWithRolesDto>> GetUsersAsync();
    Task AssignRoleAsync(string userId, string role);
    Task RemoveRoleAsync(string userId, string role);
}