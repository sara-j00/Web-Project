using System.Security.Claims;
using Application.Features.Cart.Dtos;
using Application.Features.Cart.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API_Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User not found.");
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var userId = GetUserId();
        var cart = await _cartService.GetCartAsync(userId);
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(AddToCartRequest request)
    {
        var userId = GetUserId();
        await _cartService.AddToCartAsync(userId, request.ProductId, request.Quantity);
        return Ok();
    }

    [HttpPut("items/{productId}")]
    public async Task<IActionResult> UpdateItem(int productId, UpdateCartItemRequest request)
    {
        var userId = GetUserId();
        await _cartService.UpdateQuantityAsync(userId, productId, request.Quantity);
        return NoContent();
    }

    [HttpDelete("items/{productId}")]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        var userId = GetUserId();
        await _cartService.RemoveFromCartAsync(userId, productId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userId = GetUserId();
        await _cartService.ClearCartAsync(userId);
        return NoContent();
    }
}