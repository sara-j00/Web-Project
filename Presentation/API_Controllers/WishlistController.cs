using Application.Features.Wishlist.Dtos;
using Application.Features.Wishlist.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.API_Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User not found.");

    [HttpGet]
    public async Task<ActionResult<WishlistDto>> GetWishlist()
    {
        var userId = GetUserId();
        var wishlist = await _wishlistService.GetWishlistAsync(userId);
        return Ok(wishlist);
    }

    [HttpPost("items/{productId}")]
    public async Task<IActionResult> AddItem(int productId)
    {
        try
        {
            var userId = GetUserId();
            await _wishlistService.AddToWishlistAsync(userId, productId);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("items/{productId}")]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        var userId = GetUserId();
        await _wishlistService.RemoveFromWishlistAsync(userId, productId);
        return NoContent();
    }
}