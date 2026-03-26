using System.ComponentModel.DataAnnotations;

namespace Application.Features.Admin.Dtos;

public record CreateRoleRequest(
    [Required(ErrorMessage = "Role name is required")]
    [MaxLength(100, ErrorMessage = "Name length cannot exceed 100 characters")]
    string Name
    );
