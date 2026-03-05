using Application.Abstraction;
using Application.Features.Products.Dtos;
using Domain.Entities;

namespace Application.Features.Products.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageStorageService _imageStorage;

    public ProductService(IProductRepository productRepo, IUnitOfWork unitOfWork, IImageStorageService imageStorage, ICategoryRepository categoryRepo)
    {
        _productRepo = productRepo;
        _unitOfWork = unitOfWork;
        _imageStorage = imageStorage;
        _categoryRepo = categoryRepo;
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

        await _productRepo.AddAsync(product);
        await _unitOfWork.Commit();

        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.CategoryId,
            product.Images.Select(i => i.ImageUrl).ToList());
    }

    public async Task UpdateAsync(int id, UpdateProductRequest request)
    {
        // Fetch product (includes images due to repository override – that's fine)
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null)
            throw new InvalidOperationException($"Product with id {id} not found.");

        // Verify category exists
        bool categoryExists = await _categoryRepo.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new ArgumentException($"Category with id {request.CategoryId} does not exist.");

        // Update properties
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;

        // Save changes
        await _unitOfWork.Commit();
    }

    public async Task AddImageAsync(int productId, Stream imageStream, string fileName)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
            throw new InvalidOperationException("Product not found.");

        var imageUrl = await _imageStorage.UploadAsync(imageStream, fileName);

        product.Images.Add(new ProductImage
        {
            ProductId = productId,
            ImageUrl = imageUrl
        });


        await _unitOfWork.Commit();
    }

    public async Task RemoveImageAsync(int productId, int imageId)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
            throw new InvalidOperationException("Product not found.");

        var image = product.Images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            throw new InvalidOperationException("Image not found.");

        // Optional: delete from storage first
        await _imageStorage.DeleteAsync(image.ImageUrl);

        product.Images.Remove(image);
        // If using EF Core, you may need to explicitly mark for deletion if orphan removal isn't configured
        // _productImageRepo.Remove(image); but if you have a separate repo for images

        await _unitOfWork.Commit();
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