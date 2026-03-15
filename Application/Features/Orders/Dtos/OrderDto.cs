using Domain.Enums;

namespace Application.Features.Orders.Dtos;

public record OrderDto(
    int Id,
    DateTime OrderDate,
    decimal Total,
    OrderStatus Status,
    List<OrderItemDto> Items
);