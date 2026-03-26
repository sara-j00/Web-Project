using System.ComponentModel.DataAnnotations;

namespace Application.Features.Admin.Dtos;

public record AssignRoleRequest(
    [Required(ErrorMessage = "UserId is required")]
    string UserId,
    [Required(ErrorMessage = "Role is required")]
    string Role
    );
