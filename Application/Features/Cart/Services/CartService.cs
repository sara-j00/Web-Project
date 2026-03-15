using Application.Abstraction;
using Application.Abstractions;
using Application.Features.Cart.Dtos;
using Domain.Entities;

namespace Application.Features.Cart.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepo;
    private readonly IProfileRepository _profileRepo;
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CartService(
        ICartRepository cartRepo,
        IProfileRepository profileRepo,
        IProductRepository productRepo,
        IUnitOfWork unitOfWork)
    {
        _cartRepo = cartRepo;
        _profileRepo = profileRepo;
        _productRepo = productRepo;
        _unitOfWork = unitOfWork;
    }

    private async Task<int> GetProfileIdAsync(string userId)
    {
        var profile = await _profileRepo.GetByUserIdAsync(userId);
        if (profile == null)
            throw new InvalidOperationException("Profile not found.");
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
        var profileId = await GetProfileIdAsync(userId);
        var cart = await _cartRepo.GetCartWithItemsByProfileIdAsync(profileId);
        if (cart == null)
        {
            return new CartDto(0, new List<CartItemDto>(), 0);
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
        return new CartDto(cart.Id, items, total);
    }

    public async Task AddToCartAsync(string userId, int productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");

        var profileId = await GetProfileIdAsync(userId);
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
            throw new InvalidOperationException("Product not found.");

        // Optional: check stock (if you want)
        // if (product.StockQuantity < quantity) throw ...

        var cart = await GetOrCreateCartAsync(profileId);
        var existingItem = await _cartRepo.GetCartItemAsync(cart.Id, productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            _cartRepo.UpdateItem(existingItem);
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
        }

        await _unitOfWork.Commit();
    }

    public async Task UpdateQuantityAsync(string userId, int productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");

        var profileId = await GetProfileIdAsync(userId);
        var cart = await _cartRepo.GetCartWithItemsByProfileIdAsync(profileId);
        if (cart == null)
            throw new InvalidOperationException("Cart not found.");

        var item = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
        if (item == null)
            throw new InvalidOperationException("Item not found in cart.");

        item.Quantity = quantity;
        _cartRepo.UpdateItem(item);
        await _unitOfWork.Commit();
    }

    public async Task RemoveFromCartAsync(string userId, int productId)
    {
        var profileId = await GetProfileIdAsync(userId);
        var cart = await _cartRepo.GetCartWithItemsByProfileIdAsync(profileId);
        if (cart == null) return;

        var item = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
        if (item != null)
        {
            _cartRepo.RemoveItem(item);
            await _unitOfWork.Commit();
        }
    }

    public async Task ClearCartAsync(string userId)
    {
        var profileId = await GetProfileIdAsync(userId);
        var cart = await _cartRepo.GetCartWithItemsByProfileIdAsync(profileId);
        if (cart == null) return;

        foreach (var item in cart.Items.ToList())
        {
            _cartRepo.RemoveItem(item);
        }
        await _unitOfWork.Commit();
    }
}