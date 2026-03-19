using Domain.Entities;

namespace Application.Abstraction;

public interface IWishlistRepository : IGenericRepository<Wishlist>
{
    Task<Wishlist?> GetWishlistWithItemsByProfileIdAsync(int profileId);
    Task<WishlistItem?> GetWishlistItemAsync(int wishlistId, int productId);
    void AddItem(WishlistItem item);
    void RemoveItem(WishlistItem item);
}