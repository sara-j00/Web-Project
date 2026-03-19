namespace Application.Features.Wishlist.Dtos;

public record WishlistItemDto(
    int ProductId,
    string ProductName,
    decimal Price,
    string? ImageUrl
);