using Application.Features.Products.Dtos;

namespace Application.Features.Products.Services;

public interface IProductService
{
    Task<ProductDto> CreateAsync(
        CreateProductRequest request,
        List<(Stream Stream, string FileName)>? images);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetAllAsync();
}