using Domain.Entities;
using Application.Abstractions;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Persistence.Repositories;

public class ProfileRepository : GenericRepository<Profile>, IProfileRepository
{
    public ProfileRepository(AppDbContext context) : base(context)
    {
    }
    public async Task CreateAsync(string userId, string username, string mobileNumber)
    {
        var profile = new Profile(userId, username, mobileNumber);
        await AddAsync(profile);
    }
}
