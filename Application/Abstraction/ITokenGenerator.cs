using Application.Common.Models;

namespace Application.Abstraction;

public interface ITokenGenerator
{
    string GenerateToken(UserModel user, string role);
}
