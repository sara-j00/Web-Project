using FluentValidation;

namespace Application.Features.Auth.Dtos;

public record LoginRequest
(
    string UsernameOrEmail,
    string Password
);