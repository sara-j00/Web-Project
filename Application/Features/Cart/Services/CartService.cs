using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Application.Features.Cart.Dtos;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.Cart.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepo;
    private readonly IProfileRepository _profileRepo;
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CartService> _logger;

    public CartService(
        ICartRepository cartRepo,
        IProfileRepository profileRepo,
        IProductRepository productRepo,
        IUnitOfWork unitOfWork,
        ILogger<CartService> logger)
    {
        _cartRepo = cartRepo;
        _profileRepo = profileRepo;
        _productRepo = productRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    private async Task<int> GetProfileIdAsync(string userId)
    {
        var profile = await _profileRepo.GetByUserIdAsync(userId);
        if (profile == null)
            throw new NotFoundException("Profile not found.");
        return profile.Id;
    }

    private async Task<Domain.Entities.Cart> GetOrCreateCartAsync(int profileId)
    {
        var cart = await _cartRepo.GetCartWithItemsByProfileIdAsync(profileId);
        if (cart == null)
        {
            cart = new Domain.Entities.Cart { ProfileId = profileId };
            await _cartRepo.AddAsync(cart);
            await _unitOfWork.Commit(); // save to generate Id
        }
        return cart;
    }

    public async Task<CartDto> GetCartAsync(string userId)
    {
        var profile = await _profileRepo.GetByUserIdAsync(userId);
        var cart = await _cartRepo.GetCartWithItemsByProfileIdAsync(profile.Id);
        if (cart == null)
        {
            return new CartDto(new List<CartItemDto>(), 0);
        }

        var items = cart.Items.Select(ci => new CartItemDto(
            ci.ProductId,
            ci.Product.Name,
            ci.Product.Price,
            ci.Quantity,
            ci.Quantity * ci.Product.Price,
            ci.Product.Images.FirstOrDefault()?.ImageUrl
        )).ToList();

        var total = items.Sum(i => i.TotalPrice);
        return new CartDto(items, total);
    }

    public async Task AddToCartAsync(string userId, int productId, int quantity)
    {
        _logger.LogInformation("Adding product {ProductId} to cart for user {UserId}, quantity {Quantity}", productId, userId, quantity);

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");

        var profileId = await GetProfileIdAsync(userId);
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
            throw new NotFoundException($"Product with id {productId} not found.");

        var cart = await GetOrCreateCartAsync(profileId);
        var existingItem = await _cartRepo.GetCartItemAsync(cart.Id, productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            _cartRepo.UpdateItem(existingItem);
            _logger.LogInformation("Product {ProductId} quantity increased to {NewQuantity}", productId, existingItem.Quantity);
        }
        else
        {
            var newItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity
            };
            _cartRepo.AddItem(newItem);
            _logger.LogInformation("Product {ProductId} added to cart with quantity {Quantity}", productId, quantity);
        }

        await _unitOfWork.Commit();
    }

    public async Task UpdateQuantityAsync(string userId, int productId, int quantity)
    {
        _logger.LogInformation("Updating product {ProductId} quantity to {Quantity} for user {UserId}", productId, quantity, userId);

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");

        var profileId = await GetProfileIdAsync(userId);
        var cart = await _cartRepo.GetCartWithItemsByProfileIdAsync(profileId);
        if (cart == null)
            throw new NotFoundException("Cart not found.");

        var item = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
        if (item == null)
            throw new NotFoundException($"Product {productId} not found in cart.");

        item.Quantity = quantity;
        _cartRepo.UpdateItem(item);
        await _unitOfWork.Commit();

        _logger.LogInformation("Product {ProductId} quantity updated to {Quantity}", productId, quantity);
    }

    public async Task RemoveFromCartAsync(string userId, int productId)
    {
        _logger.LogInformation("Removing product {ProductId} from cart for user {UserId}", productId, userId);

        var profileId = await GetProfileIdAsync(userId);
        var cart = await _cartRepo.GetCartWithItemsByProfileIdAsync(profileId);
        if (cart == null) return; // nothing to remove

        var item = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
        if (item != null)
        {
            _cartRepo.RemoveItem(item);
            await _unitOfWork.Commit();
            _logger.LogInformation("Product {ProductId} removed from cart", productId);
        }
        else
        {
            _logger.LogWarning("Attempted to remove product {ProductId} not in cart for user {UserId}", productId, userId);
        }
    }

    public async Task ClearCartAsync(string userId)
    {
        _logger.LogInformation("Clearing cart for user {UserId}", userId);

        var profileId = await GetProfileIdAsync(userId);
        var cart = await _cartRepo.GetCartWithItemsByProfileIdAsync(profileId);
        if (cart == null) return;

        var items = cart.Items.ToList();
        foreach (var item in items)
            _cartRepo.RemoveItem(item);

        await _unitOfWork.Commit();
        _logger.LogInformation("Cart cleared for user {UserId}, {Count} items removed", userId, items.Count);
    }
}