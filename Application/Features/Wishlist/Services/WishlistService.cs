using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions; // NotFoundException
using Application.Features.Wishlist.Dtos;
using Application.Features.Wishlist.Services;
using Domain.Entities;
using Microsoft.Extensions.Logging;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepo;
    private readonly IProfileRepository _profileRepo;
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WishlistService> _logger;

    public WishlistService(
        IWishlistRepository wishlistRepo,
        IProfileRepository profileRepo,
        IProductRepository productRepo,
        IUnitOfWork unitOfWork,
        ILogger<WishlistService> logger)
    {
        _wishlistRepo = wishlistRepo;
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

    private async Task<Wishlist> GetOrCreateWishlistAsync(int profileId)
    {
        var wishlist = await _wishlistRepo.GetWishlistWithItemsByProfileIdAsync(profileId);
        if (wishlist == null)
        {
            wishlist = new Wishlist { ProfileId = profileId };
            await _wishlistRepo.AddAsync(wishlist);
            await _unitOfWork.Commit(); // save to generate Id
        }
        return wishlist;
    }

    public async Task<WishlistDto> GetWishlistAsync(string userId)
    {
        var profileId = await GetProfileIdAsync(userId);
        var wishlist = await _wishlistRepo.GetWishlistWithItemsByProfileIdAsync(profileId);
        if (wishlist == null)
            return new WishlistDto(new List<WishlistItemDto>());

        var items = wishlist.Items.Select(wi => new WishlistItemDto(
            wi.ProductId,
            wi.Product.Name,
            wi.Product.Price,
            wi.Product.Images.FirstOrDefault()?.ImageUrl
        )).ToList();

        return new WishlistDto(items);
    }

    public async Task AddToWishlistAsync(string userId, int productId)
    {
        _logger.LogInformation("Adding product {ProductId} to wishlist for user {UserId}", productId, userId);

        var profileId = await GetProfileIdAsync(userId);
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
            throw new NotFoundException($"Product with id {productId} not found.");

        var wishlist = await GetOrCreateWishlistAsync(profileId);
        var existingItem = await _wishlistRepo.GetWishlistItemAsync(wishlist.Id, productId);

        if (existingItem != null)
        {
            _logger.LogInformation("Product {ProductId} already in wishlist for user {UserId}", productId, userId);
            return; // already exists, do nothing
        }

        var newItem = new WishlistItem
        {
            WishlistId = wishlist.Id,
            ProductId = productId
        };
        _wishlistRepo.AddItem(newItem);
        await _unitOfWork.Commit();

        _logger.LogInformation("Product {ProductId} added to wishlist for user {UserId}", productId, userId);
    }

    public async Task RemoveFromWishlistAsync(string userId, int productId)
    {
        _logger.LogInformation("Removing product {ProductId} from wishlist for user {UserId}", productId, userId);

        var profileId = await GetProfileIdAsync(userId);
        var wishlist = await _wishlistRepo.GetWishlistWithItemsByProfileIdAsync(profileId);
        if (wishlist == null)
        {
            _logger.LogWarning("Attempted to remove product from non-existent wishlist for user {UserId}", userId);
            return;
        }

        var item = wishlist.Items.FirstOrDefault(wi => wi.ProductId == productId);
        if (item != null)
        {
            _wishlistRepo.RemoveItem(item);
            await _unitOfWork.Commit();
            _logger.LogInformation("Product {ProductId} removed from wishlist for user {UserId}", productId, userId);
        }
        else
        {
            _logger.LogWarning("Attempted to remove product {ProductId} not in wishlist for user {UserId}", productId, userId);
        }
    }
}