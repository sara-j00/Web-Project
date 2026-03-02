using Application.Features.Auth.Dtos;

namespace Application.Features.Auth.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task LoginAsync(LoginRequest request);
    Task LogoutAsync();
    //Task ChangePasswordAsync(ChangePasswordRequest request);
}
