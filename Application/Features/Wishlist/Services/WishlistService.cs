using Application.Abstraction;
using Application.Abstractions;
using Application.Features.Wishlist.Dtos;
using Application.Features.Wishlist.Services;
using Domain.Entities;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepo;
    private readonly IProfileRepository _profileRepo;
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork _unitOfWork;

    public WishlistService(
        IWishlistRepository wishlistRepo,
        IProfileRepository profileRepo,
        IProductRepository productRepo,
        IUnitOfWork unitOfWork)
    {
        _wishlistRepo = wishlistRepo;
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
            return new WishlistDto(0, new List<WishlistItemDto>());

        var items = wishlist.Items.Select(wi => new WishlistItemDto(
            wi.ProductId,
            wi.Product.Name,
            wi.Product.Price,
            wi.Product.Images.FirstOrDefault()?.ImageUrl
        )).ToList();

        return new WishlistDto(wishlist.Id, items);
    }

    public async Task AddToWishlistAsync(string userId, int productId)
    {
        var profileId = await GetProfileIdAsync(userId);
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
            throw new InvalidOperationException("Product not found.");

        var wishlist = await GetOrCreateWishlistAsync(profileId);
        var existingItem = await _wishlistRepo.GetWishlistItemAsync(wishlist.Id, productId);

        if (existingItem != null)
            return; // already in wishlist, do nothing (or could throw)

        var newItem = new WishlistItem
        {
            WishlistId = wishlist.Id,
            ProductId = productId
        };
        _wishlistRepo.AddItem(newItem);
        await _unitOfWork.Commit();
    }

    public async Task RemoveFromWishlistAsync(string userId, int productId)
    {
        var profileId = await GetProfileIdAsync(userId);
        var wishlist = await _wishlistRepo.GetWishlistWithItemsByProfileIdAsync(profileId);
        if (wishlist == null) return;

        var item = wishlist.Items.FirstOrDefault(wi => wi.ProductId == productId);
        if (item != null)
        {
            _wishlistRepo.RemoveItem(item);
            await _unitOfWork.Commit();
        }
    }
}