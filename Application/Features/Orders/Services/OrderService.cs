using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Application.Features.Orders.Dtos;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly ICartRepository _cartRepo;
    private readonly IProfileRepository _profileRepo;
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService; // mock
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepo,
        ICartRepository cartRepo,
        IProfileRepository profileRepo,
        IProductRepository productRepo,
        IUnitOfWork unitOfWork,
        IPaymentService paymentService,
        ILogger<OrderService> logger)
    {
        _orderRepo = orderRepo;
        _cartRepo = cartRepo;
        _profileRepo = profileRepo;
        _productRepo = productRepo;
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _logger = logger;
    }



    private static OrderDto MapToDto(Order order, string userName)
    {
        var items = order.Items.Select(i => new OrderItemDto(
            i.ProductId,
            i.ProductName,
            i.UnitPrice,
            i.Quantity,
            i.UnitPrice * i.Quantity
        )).ToList();
        return new OrderDto(order.Id, order.OrderDate, order.Total, order.Status, items, userName);
    }

    public async Task<OrderDto> PlaceOrderAsync(string userId)
    {
        _logger.LogInformation("Placing order for user {UserId}", userId);

        await _unitOfWork.StartTransaction();

        try
        {
            var profile = await _profileRepo.GetByUserIdAsync(userId);
            if (profile == null)
                throw new NotFoundException("User profile not found."); // 404

            // 1. Get cart with items
            var cart = await _cartRepo.GetCartWithItemsByProfileIdAsync(profile.Id);
            if (cart == null || !cart.Items.Any())
                throw new ArgumentException("Cart is empty."); // 400

            // 2. Validate stock and prepare order items
            var orderItems = new List<OrderItem>();
            foreach (var cartItem in cart.Items)
            {
                var product = await _productRepo.GetByIdAsync(cartItem.ProductId);
                if (product == null)
                    throw new NotFoundException($"Product {cartItem.ProductId} not found."); // 404

                if (product.Stock < cartItem.Quantity)
                {
                    _logger.LogWarning("Insufficient stock for product {ProductId}, requested {Requested}, available {Available}", product.Id, cartItem.Quantity, product.Stock);
                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}.");

                }
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
                ProfileId = profile.Id,
                Total = total,
                Status = OrderStatus.Pending,
                Items = orderItems
            };

            _logger.LogInformation("Order created with ID {OrderId}, total {Total}", order.Id, order.Total);

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
            return MapToDto(order, profile.Username);
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
    {
        var profile = await _profileRepo.GetByUserIdAsync(userId);
        if (profile == null)
            throw new NotFoundException("User profile not found."); // 404
        var orders = await _orderRepo.GetOrdersByProfileIdAsync(profile.Id);
        return orders.Select(o => MapToDto(o, profile.Username));
    }

    public async Task<OrderDto> GetOrderAsync(int orderId, string userId)
    {
        var profile = await _profileRepo.GetByUserIdAsync(userId);
        if (profile == null)
            throw new NotFoundException("User profile not found."); // 404
        var order = await _orderRepo.GetOrderWithItemsAsync(orderId, profile.Id);
        if (order == null)
            throw new NotFoundException("Order not found."); // 404
        return MapToDto(order, profile.Username);
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepo.GetAllOrdersWithItemsAsync();
        var result = new List<OrderDto>();

        foreach (var order in orders)
        {
            var profile = await _profileRepo.GetByIdAsync(order.ProfileId);
            var userName = profile?.Username ?? "Unknown";
            result.Add(MapToDto(order, userName));
        }
        return result;
    }

    public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
    {
        // Validate status string
        if (!Enum.TryParse<OrderStatus>(newStatus, true, out var status))
            throw new ArgumentException($"Invalid order status: {newStatus}");

        var order = await _orderRepo.GetOrderWithItemsByIdAsync(orderId);
        if (order == null)
            throw new NotFoundException($"Order with ID {orderId} not found."); //404

        // Optional: add business rules (e.g., cannot change status from Shipped to Pending)
        order.Status = status;
        // No need to call update – entity is tracked
        await _unitOfWork.Commit();
    }
}