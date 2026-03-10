using Application.Abstraction;
using Application.Abstractions;
using Application.Common.Models;
using Application.Features.Auth.Dtos;

namespace Application.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IProfileRepository profileRepository,
        IUnitOfWork unitOfWork,
        ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        await _unitOfWork.StartTransaction();

        try
        {
            // 1. Check uniqueness (optional, can also be done by Identity)
            if (await _userRepository.EmailExistsAsync(request.Email))
                throw new Exception("Email already exists.");
            if (await _userRepository.UsernameExistsAsync(request.Username))
                throw new Exception("Username already exists.");

            // 2. Create Identity user
            var userId = await _userRepository.CreateAsync(
                request.Email,
                request.Username,
                request.Password
            );

            // 3. Fetch the created user (to get full details)
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User creation failed.");

            // 4. Create profile
            await _profileRepository.CreateAsync(
                userId,
                request.Username,
                request.MobileNumber
            );

            // 5. Assign default role "Customer"
            const string role = "Customer";
            await _userRepository.AddToRoleAsync(userId, role);

            // 6. Commit transaction
            await _unitOfWork.Commit();
            await _unitOfWork.CommitTransaction();

            // 7. Generate token
            var token = _tokenGenerator.GenerateToken(user, role);

            return new AuthResponse(token, user.Email, role);
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // 1. Find user by email or username
        UserModel? user;
        if (request.UsernameOrEmail.Contains("@"))
            user = await _userRepository.GetUserByEmailAsync(request.UsernameOrEmail);
        else
            user = await _userRepository.GetUserByUsernameAsync(request.UsernameOrEmail);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        // 2. Check password
        bool valid = await _userRepository.CheckPasswordAsync(user.Id, request.Password);
        if (!valid)
            throw new UnauthorizedAccessException("Invalid credentials");

        // 3. Get roles
        var roles = await _userRepository.GetRolesAsync(user.Id);
        var role = roles.FirstOrDefault() ?? "Customer";

        // 4. Generate token
        var token = _tokenGenerator.GenerateToken(user, role);

        return new AuthResponse(token, user.Email, role);
    }

    public Task LogoutAsync() => Task.CompletedTask; // JWT is stateless
}
