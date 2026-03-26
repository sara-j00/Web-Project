namespace Application.Features.Cart.Dtos;

public record CartDto(
    List<CartItemDto> Items,
    decimal Total
);