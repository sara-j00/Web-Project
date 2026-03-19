using Application.Features.Wishlist.Dtos;

namespace Application.Features.Wishlist.Services;

public interface IWishlistService
{
    Task<WishlistDto> GetWishlistAsync(string userId);
    Task AddToWishlistAsync(string userId, int productId);
    Task RemoveFromWishlistAsync(string userId, int productId);
}