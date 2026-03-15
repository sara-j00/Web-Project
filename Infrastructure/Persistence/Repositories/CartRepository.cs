using Application.Abstraction;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    private readonly DbContext _context;

    public CartRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Cart?> GetCartWithItemsByProfileIdAsync(int profileId)
    {
        return await _context.Set<Cart>()
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(c => c.ProfileId == profileId);
    }

    public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
    {
        return await _context.Set<CartItem>()
            .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
    }

    public void AddItem(CartItem item)
    {
        _context.Set<CartItem>().Add(item);
    }

    public void UpdateItem(CartItem item)
    {
        _context.Set<CartItem>().Update(item);
    }

    public void RemoveItem(CartItem item)
    {
        _context.Set<CartItem>().Remove(item);
    }
}