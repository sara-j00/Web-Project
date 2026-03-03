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

        await _unitOfWork.StartTransaction();

        try
        {
        var userId = await _userRepository.CreateAsync(
            request.Email,
            request.Username,
            request.Password
        );

            await _profileRepository.CreateAsync(
                userId,
                request.Username,
                request.MobileNumber
            );

        // 3. Persist profile (Identity already saved internally)
        await _unitOfWork.Commit();
            await _unitOfWork.CommitTransaction();
    }

        catch
    {
            await _unitOfWork.Rollback();
            throw;
    }
    }

    //public Task ChangePasswordAsync(ChangePasswordRequest request)
    //{
    //    throw new NotImplementedException();
    //}

    public async Task LoginAsync(LoginRequest request)
    {
        string? userId;

        if (request.UsernameOrEmail.Contains("@"))
            userId = await _userRepository.GetUserIdByEmailAsync(request.UsernameOrEmail);
        else
            userId = await _userRepository.GetUserIdByUsernameAsync(request.UsernameOrEmail);

        if (userId == null)
            throw new Exception("Invalid credentials");

        bool valid = await _userRepository.CheckPasswordAsync(userId, request.Password);
        if (!valid)
            throw new Exception("Invalid credentials");

        await _userRepository.SignInAsync(userId);
    }

    public async Task LogoutAsync()
    {
        await _userRepository.SignOutAsync();
    }
}
