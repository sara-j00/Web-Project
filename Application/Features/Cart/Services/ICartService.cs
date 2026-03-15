using Application.Features.Cart.Dtos;

namespace Application.Features.Cart.Services;

public interface ICartService
{
    Task<CartDto> GetCartAsync(string userId);
    Task AddToCartAsync(string userId, int productId, int quantity);
    Task UpdateQuantityAsync(string userId, int productId, int quantity);
    Task RemoveFromCartAsync(string userId, int productId);
    Task ClearCartAsync(string userId);
}
