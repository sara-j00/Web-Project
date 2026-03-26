using System.ComponentModel.DataAnnotations;

namespace Application.Features.Auth.Dtos;

public record ForgotPasswordRequest(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email
    );
