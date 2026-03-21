using Application.Features.Auth.Dtos;

namespace Application.Features.Auth.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task ForgotPasswordAsync(string email, string origin);
    Task ResetPasswordAsync(ResetPasswordRequest request);
    Task ChangePasswordAsync(string userId, ChangePasswordRequest request);
}
