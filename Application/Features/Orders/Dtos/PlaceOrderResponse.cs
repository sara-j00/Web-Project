using Domain.Enums;

public record PlaceOrderResponse(int OrderId, decimal Total, OrderStatus Status);