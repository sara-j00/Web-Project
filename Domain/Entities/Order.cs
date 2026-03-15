using Domain.Enums;

namespace Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // Navigation
    public Profile Profile { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}