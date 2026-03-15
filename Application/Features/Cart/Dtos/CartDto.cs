namespace Application.Features.Cart.Dtos;

public record CartDto(
    int Id,
    List<CartItemDto> Items,
    decimal Total
);