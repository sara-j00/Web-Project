using Application.Features.Auth.Dtos;

namespace Application.Features.Auth.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task LoginAsync(LoginRequest request);
    Task LogoutAsync(string userId);
    Task ChangePasswordAsync(ChangePasswordRequest request);
}
