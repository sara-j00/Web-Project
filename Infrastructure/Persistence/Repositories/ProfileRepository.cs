using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

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
    public async Task<Profile?> GetByUserIdAsync(string userId)
    {
        return await _context.Set<Profile>().FirstOrDefaultAsync(p => p.UserId == userId);
    }
}
