using Application.Abstraction;
using Application.Abstractions;
using Application.Common.Models;
using Application.Exceptions;
using Application.Features.Auth.Dtos;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IProfileRepository profileRepository,
        IUnitOfWork unitOfWork,
        ITokenGenerator tokenGenerator,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
        _tokenGenerator = tokenGenerator;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        await _unitOfWork.StartTransaction();

        try
        {
            // 1. Check uniqueness
            if (await _userRepository.EmailExistsAsync(request.Email))
                throw new ConflictException("Email already exists.");
            if (await _userRepository.UsernameExistsAsync(request.Username))
                throw new ConflictException("Username already exists.");

            // 2. Create Identity user
            var userId = await _userRepository.CreateAsync(
                request.Email,
                request.Username,
                request.Password);

            // 3. Fetch created user (to get full details)
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User creation failed.");

            // 4. Create profile and get its id
            await _profileRepository.CreateAsync(userId, request.Username, request.MobileNumber);
                        
            // 5. Assign default role
            const string role = "Customer";
            await _userRepository.AddToRoleAsync(userId, role);

            // 6. Commit transaction
            await _unitOfWork.Commit();
            await _unitOfWork.CommitTransaction();

            var profile = await _profileRepository.GetByUserIdAsync(userId);
            var profileId = profile?.Id ?? 0;

            _logger.LogInformation("User registered successfully. Email: {Email}, ProfileId: {ProfileId}", user.Email, profileId);


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
        _logger.LogInformation("Login attempt for {UsernameOrEmail}", request.UsernameOrEmail);

        UserModel? user;
        if (request.UsernameOrEmail.Contains("@"))
            user = await _userRepository.GetUserByEmailAsync(request.UsernameOrEmail);
        else
            user = await _userRepository.GetUserByUsernameAsync(request.UsernameOrEmail);

        if (user == null)
        {
            _logger.LogWarning("Login failed: user not found for {UsernameOrEmail}", request.UsernameOrEmail);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        bool valid = await _userRepository.CheckPasswordAsync(user.Id, request.Password);
        if (!valid)
        {
            _logger.LogWarning("Login failed: invalid password for user {UserId}", user.Id);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var roles = await _userRepository.GetRolesAsync(user.Id);
        var role = roles.FirstOrDefault() ?? "Customer";

        var token = _tokenGenerator.GenerateToken(user, role);

        _logger.LogInformation("User {UserId} logged in successfully", user.Id);
        return new AuthResponse(token, user.Email, role);
    }

    public Task LogoutAsync() => Task.CompletedTask;

    public async Task ForgotPasswordAsync(string email, string origin)
    {
        _logger.LogInformation("Password reset requested for {Email}", email);

        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            // Do not leak existence; log a warning for monitoring
            _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
            return;
        }

        var token = await _userRepository.GeneratePasswordResetTokenAsync(user.Id);
        var resetLink = $"{origin}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

        var subject = "Password Reset";
        var body = $@"
        <p>Please reset your password by clicking the link below:</p>
        <p><a href='{resetLink}'>Reset Password</a></p>
        <p>If you didn't request this, please ignore this email.</p>";

        await _emailService.SendEmailAsync(email, subject, body);
        _logger.LogInformation("Password reset email sent to {Email}", email);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        _logger.LogInformation("Password reset attempt for {Email}", request.Email);

        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null)
            throw new InvalidOperationException("Invalid request.");

        var succeeded = await _userRepository.ResetPasswordAsync(user.Id, request.Token, request.NewPassword);
        if (!succeeded)
        {
            _logger.LogWarning("Password reset failed for {Email} – token invalid or expired", request.Email);
            throw new InvalidOperationException("Password reset failed. Token may be invalid or expired.");
        }

        _logger.LogInformation("Password reset succeeded for {Email}", request.Email);
    }

    public async Task ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        _logger.LogInformation("Password change attempt for user {UserId}", userId);

        var succeeded = await _userRepository.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
        if (!succeeded)
        {
            _logger.LogWarning("Password change failed for user {UserId} – current password incorrect", userId);
            throw new InvalidOperationException("Current password is incorrect or new password is invalid.");
        }

        _logger.LogInformation("Password changed successfully for user {UserId}", userId);
    }
}