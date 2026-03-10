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
        try
        {
            var product = await _productService.GetByIdAsync(id);
            return Ok(product);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Create([FromForm] CreateProductRequest request, [FromForm] List<IFormFile>? images) 
    {
        try
        {
            // Convert IFormFile to streams (use extension method)
            var imageStreams = images?.Select(f => (f.OpenReadStream(), f.FileName)).ToList();
            var product = await _productService.CreateAsync(request, imageStreams);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateProductRequest request)
    {
        try
        {
            await _productService.UpdateAsync(id, request);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //[HttpDelete("{id}")]
    //[Authorize(Roles = "Admin")]
    //public async Task<IActionResult> Delete(int id)
    //{
    //    try
    //    {
    //        await _productService.DeleteAsync(id);
    //        return NoContent();
    //    }
    //    catch (InvalidOperationException ex)
    //    {
    //        return NotFound(ex.Message);
    //    }
    //}

    [HttpPost("{id}/images")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddImage(int id, IFormFile image)
    {
        if (image == null || image.Length == 0)
            return BadRequest("No image uploaded.");

        try
        {
            using var stream = image.OpenReadStream();
            await _productService.AddImageAsync(id, stream, image.FileName);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}/images/{imageId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveImage(int id, int imageId)
    {
        try
        {
            await _productService.RemoveImageAsync(id, imageId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

}
