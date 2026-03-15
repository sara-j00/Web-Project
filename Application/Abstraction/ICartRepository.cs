using Domain.Entities;

namespace Application.Abstraction;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart?> GetCartWithItemsByProfileIdAsync(int profileId);
    Task<CartItem?> GetCartItemAsync(int cartId, int productId);
    void AddItem(CartItem item);
    void UpdateItem(CartItem item);
    void RemoveItem(CartItem item);
}