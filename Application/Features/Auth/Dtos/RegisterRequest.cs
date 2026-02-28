using FluentValidation;

namespace Application.Features.Auth.Dtos;

public record RegisterRequest
(
    string Email,
    string Username,
    string Password, 
    string MobileNumber
);

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(30);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.MobileNumber)
            .NotEmpty()
            .MaximumLength(20);
    }
}