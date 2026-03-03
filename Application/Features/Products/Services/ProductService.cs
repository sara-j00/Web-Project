using Application.Abstraction;
using Application.Features.Products.Dtos;

namespace Application.Features.Products.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageStorageService _imageStorage;

    public ProductService(IProductRepository productRepo, IUnitOfWork unitOfWork, IImageStorageService imageStorage)
    {
        _productRepo = productRepo;
        _unitOfWork = unitOfWork;
        _imageStorage = imageStorage;
    }

    public async Task<ProductDto> CreateAsync(
        CreateProductRequest request,
        List<(Stream Stream, string FileName)>? images)
    {
        var product = new Domain.Entities.Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId
        };

        if (images != null && images.Count > 0)
        {
            foreach (var image in images)
            {
                var imageUrl = await _imageStorage.UploadAsync(image.Stream, image.FileName);

                product.Images.Add(new Domain.Entities.ProductImage
                {
                    ImageUrl = imageUrl
                });
            }
        }

        await _productRepo.CreateAsync(product);
        await _unitOfWork.Commit();

        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.CategoryId,
            product.Images.Select(i => i.ImageUrl).ToList());
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var p = await _productRepo.GetByIdAsync(id);
        if (p == null) return null; 
        return new ProductDto( p.Id, p.Name, p.Description, p.Price, p.CategoryId, p.Images.Select(i => i.ImageUrl).ToList());
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepo.GetAllAsync();
        return products.Select(p => new ProductDto( p.Id, p.Name, p.Description, p.Price, p.CategoryId, p.Images.Select(i => i.ImageUrl).ToList()));
    }
}