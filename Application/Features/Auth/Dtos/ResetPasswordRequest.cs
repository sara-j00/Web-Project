namespace Application.Features.Auth.Dtos;

public record ResetPasswordRequest(string Email, string Token, string NewPassword);