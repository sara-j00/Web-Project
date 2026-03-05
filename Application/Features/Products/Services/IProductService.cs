using Application.Features.Products.Dtos;

namespace Application.Features.Products.Services;

public interface IProductService
{
    Task<ProductDto> CreateAsync(CreateProductRequest request, List<(Stream Stream, string FileName)>? images);
    Task UpdateAsync(int id, UpdateProductRequest request);
    Task AddImageAsync(int productId, Stream imageStream, string fileName);
    Task RemoveImageAsync(int productId, int imageId);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetAllAsync();
}