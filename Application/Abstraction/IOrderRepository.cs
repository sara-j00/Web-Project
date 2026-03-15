using Domain.Entities;

namespace Application.Abstraction;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByProfileIdAsync(int profileId);
    Task<Order?> GetOrderWithItemsAsync(int orderId, int profileId);
}