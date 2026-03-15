using Application.Abstraction;
using Application.Abstractions;
using Application.Features.Orders.Dtos;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Orders.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly ICartRepository _cartRepo;
    private readonly IProfileRepository _profileRepo;
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService; // mock

    public OrderService(
        IOrderRepository orderRepo,
        ICartRepository cartRepo,
        IProfileRepository profileRepo,
        IProductRepository productRepo,
        IUnitOfWork unitOfWork,
        IPaymentService paymentService)
    {
        _orderRepo = orderRepo;
        _cartRepo = cartRepo;
        _profileRepo = profileRepo;
        _productRepo = productRepo;
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
    }

    private async Task<int> GetProfileIdAsync(string userId)
    {
        var profile = await _profileRepo.GetByUserIdAsync(userId);
        if (profile == null)
            throw new InvalidOperationException("Profile not found.");
        return profile.Id;
    }

    public async Task<OrderDto> PlaceOrderAsync(string userId)
    {
        await _unitOfWork.StartTransaction();

        try
        {
            var profileId = await GetProfileIdAsync(userId);

            // 1. Get cart with items
            var cart = await _cartRepo.GetCartWithItemsByProfileIdAsync(profileId);
            if (cart == null || !cart.Items.Any())
                throw new InvalidOperationException("Cart is empty.");

            // 2. Validate stock and prepare order items
            var orderItems = new List<OrderItem>();
            foreach (var cartItem in cart.Items)
            {
                var product = await _productRepo.GetByIdAsync(cartItem.ProductId);
                if (product == null)
                    throw new InvalidOperationException($"Product {cartItem.ProductId} not found.");
                if (product.Stock < cartItem.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}.");

                // Deduct stock
                product.Stock -= cartItem.Quantity;
                _productRepo.Update(product);

                // Snapshot
                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = cartItem.Quantity
                });
            }

            // 3. Calculate total
            var total = orderItems.Sum(oi => oi.UnitPrice * oi.Quantity);

            // 4. Create order
            var order = new Order
            {
                ProfileId = profileId,
                Total = total,
                Status = OrderStatus.Pending,
                Items = orderItems
            };
            await _orderRepo.AddAsync(order);

            // 5. Process payment (mock)
            var paymentSuccess = await _paymentService.ProcessPaymentAsync(total);
            if (paymentSuccess)
                order.Status = OrderStatus.Paid;

            // 6. Clear cart
            foreach (var item in cart.Items.ToList())
                _cartRepo.RemoveItem(item);

            await _unitOfWork.Commit();
            await _unitOfWork.CommitTransaction();

            // 7. Return DTO
            return MapToDto(order);
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
    {
        var profileId = await GetProfileIdAsync(userId);
        var orders = await _orderRepo.GetOrdersByProfileIdAsync(profileId);
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto> GetOrderAsync(int orderId, string userId)
    {
        var profileId = await GetProfileIdAsync(userId);
        var order = await _orderRepo.GetOrderWithItemsAsync(orderId, profileId);
        if (order == null)
            throw new InvalidOperationException("Order not found.");
        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order order)
    {
        var items = order.Items.Select(i => new OrderItemDto(
            i.ProductId,
            i.ProductName,
            i.UnitPrice,
            i.Quantity,
            i.UnitPrice * i.Quantity
        )).ToList();
        return new OrderDto(order.Id, order.OrderDate, order.Total, order.Status, items);
    }
}