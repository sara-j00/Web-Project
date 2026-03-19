using Application.Features.Admin.Dtos; // or define simple return types

namespace Application.Abstraction;

public interface IAdminRepository
{
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task CreateRoleAsync(string roleName);
    Task DeleteRoleAsync(string roleId);
    Task<IEnumerable<UserWithRolesDto>> GetUsersAsync();
    Task AssignRoleAsync(string userId, string role);
    Task RemoveRoleAsync(string userId, string role);
}