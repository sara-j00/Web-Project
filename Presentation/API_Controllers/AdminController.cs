using Application.Features.Admin.Dtos;
using Application.Features.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API_Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // Role management
    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
    {
        var roles = await _adminService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole(CreateRoleRequest request)
    {
        try
        {
            await _adminService.CreateRoleAsync(request.Name);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("roles/{roleId}")]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        try
        {
            await _adminService.DeleteRoleAsync(roleId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // User management
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserWithRolesDto>>> GetUsers()
    {
        var users = await _adminService.GetUsersAsync();
        return Ok(users);
    }

    [HttpPost("users/assign-role")]
    public async Task<IActionResult> AssignRole(AssignRoleRequest request)
    {
        try
        {
            await _adminService.AssignRoleAsync(request.UserId, request.Role);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("users/remove-role")]
    public async Task<IActionResult> RemoveRole(AssignRoleRequest request)
    {
        try
        {
            await _adminService.RemoveRoleAsync(request.UserId, request.Role);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}