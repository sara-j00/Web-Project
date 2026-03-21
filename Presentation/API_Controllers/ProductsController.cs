using Application.Features.Products.Dtos;
using Application.Features.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API_Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }


    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Create([FromForm] CreateProductRequest request, [FromForm] List<IFormFile>? images)
    {
        // Convert IFormFile to streams (use extension method)
        var imageStreams = images?.Select(f => (f.OpenReadStream(), f.FileName)).ToList();
        var product = await _productService.CreateAsync(request, imageStreams);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateProductRequest request)
    {
        await _productService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/images")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddImage(int id, IFormFile image)
    {
        if (image == null || image.Length == 0)
            return BadRequest("No image uploaded.");

        using var stream = image.OpenReadStream();
        await _productService.AddImageAsync(id, stream, image.FileName);
        return Ok();
    }

    [HttpDelete("{id}/images/{imageId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveImage(int id, int imageId)
    {
        await _productService.RemoveImageAsync(id, imageId);
        return NoContent();
    }

}
