namespace Application.Features.Cart.Dtos;

public record CartItemDto(
    int ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice,
    string? ImageUrl
);
