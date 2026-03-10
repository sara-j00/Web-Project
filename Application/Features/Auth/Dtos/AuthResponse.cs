namespace Application.Features.Auth.Dtos;

public record AuthResponse
(
    string Token,
    string Email,
    string Role
);
