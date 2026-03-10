using Application.Features.Auth.Dtos;

namespace Application.Features.Auth.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    //Task ChangePasswordAsync(ChangePasswordRequest request);
}
