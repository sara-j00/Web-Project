using Application.Abstraction;
using Application.Abstractions;
using Application.Features.Auth.Dtos;

namespace Application.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IProfileRepository profileRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        // 1. Create Identity user
        var userId = await _userRepository.CreateAsync(
            request.Email,
            request.Username,
            request.Password
        );

        // 2. Queue profile creation
        await _profileRepository.CreateAsync(
            userId,
            request.Username,
            request.MobileNumber
        );

        // 3. Persist profile (Identity already saved internally)
        await _unitOfWork.Commit();
    }

    public Task ChangePasswordAsync(ChangePasswordRequest request)
    {
        throw new NotImplementedException();
    }

    public Task LoginAsync(LoginRequest request)
    {
        throw new NotImplementedException();
    }

    public Task LogoutAsync(string userId)
    {
        throw new NotImplementedException();
    }

}
