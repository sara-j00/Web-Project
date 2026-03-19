namespace Application.Features.Admin.Dtos;

public record UserWithRolesDto(
    string Id,
    string Email,
    string UserName,
    List<string> Roles
);