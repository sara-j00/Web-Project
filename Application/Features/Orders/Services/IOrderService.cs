using Application.Features.Orders.Dtos;

namespace Application.Features.Orders.Services;

public interface IOrderService
{
    Task<OrderDto> PlaceOrderAsync(string userId);
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId);
    Task<OrderDto> GetOrderAsync(int orderId, string userId);
}