using Domain.Entities;

namespace Application.Abstractions;

public interface IProfileRepository
{
    Task CreateAsync(string userId, string username, string mobileNumber);
}
