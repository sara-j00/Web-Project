using Application.Abstraction;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class WishlistRepository : GenericRepository<Wishlist>, IWishlistRepository
{
    public WishlistRepository(AppDbContext context) : base(context) { }

    public async Task<Wishlist?> GetWishlistWithItemsByProfileIdAsync(int profileId)
    {
        return await _context.Set<Wishlist>()
            .Include(w => w.Items)
                .ThenInclude(wi => wi.Product)
                    .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(w => w.ProfileId == profileId);
    }

    public async Task<WishlistItem?> GetWishlistItemAsync(int wishlistId, int productId)
    {
        return await _context.Set<WishlistItem>()
            .FirstOrDefaultAsync(wi => wi.WishlistId == wishlistId && wi.ProductId == productId);
    }

    public void AddItem(WishlistItem item) => _context.Set<WishlistItem>().Add(item);
    public void RemoveItem(WishlistItem item) => _context.Set<WishlistItem>().Remove(item);
}