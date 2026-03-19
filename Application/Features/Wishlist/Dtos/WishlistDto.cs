namespace Application.Features.Wishlist.Dtos;

public record WishlistDto(
    int Id,
    List<WishlistItemDto> Items
);