using System.ComponentModel.DataAnnotations;

namespace Application.Features.Auth.Dtos;


public record ChangePasswordRequest
{
    [Required(ErrorMessage = "Current password is required")]
    [MinLength(6, ErrorMessage = "Current password must be at least 6 characters")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "New password is required")]
    [MinLength(6, ErrorMessage = "New password must be at least 6 characters")]
    public string NewPassword { get; set; }


    [Required(ErrorMessage = "Confirm new password is required")]
    [Compare(nameof(NewPassword), ErrorMessage = "Confirm new password must match new password")]
    public string ConfirmNewPassword { get; set; }
};