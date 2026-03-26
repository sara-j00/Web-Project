using System.ComponentModel.DataAnnotations;

namespace Application.Features.Auth.Dtos;

public record RegisterRequest
(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,

    [Required(ErrorMessage = "Username is required")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
    [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    string Username,
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    string Password, 
    
    [Required(ErrorMessage = "Mobile number is required")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "mobile must contain only digits")]
    [StringLength(15, MinimumLength = 10, ErrorMessage = "mobile number must be between 10 and 15 digits")]
    string MobileNumber
);
