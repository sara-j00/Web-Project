using Application.Abstraction;
using Application.Exceptions; // NotFoundException, ConflictException
using Application.Features.Products.Dtos;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageStorageService _imageStorage;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepo,
        IUnitOfWork unitOfWork,
        IImageStorageService imageStorage,
        ICategoryRepository categoryRepo,
        ILogger<ProductService> logger)
    {
        _productRepo = productRepo;
        _unitOfWork = unitOfWork;
        _imageStorage = imageStorage;
        _categoryRepo = categoryRepo;
        _logger = logger;
    }

    public async Task<ProductDto> CreateAsync(
        CreateProductRequest request,
        List<(Stream Stream, string FileName)>? images)
    {
        _logger.LogInformation("Creating product: {Name}, price {Price}", request.Name, request.Price);

        // Validate category existence (optional – could be done by service rule)
        var categoryExists = await _categoryRepo.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new NotFoundException($"Category with id {request.CategoryId} does not exist.");

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId
        };

        if (images != null && images.Count > 0)
        {
            foreach (var image in images)
            {
                var imageUrl = await _imageStorage.UploadAsync(image.Stream, image.FileName);
                product.Images.Add(new ProductImage { ImageUrl = imageUrl });
            }
        }

        await _productRepo.AddAsync(product);
        await _unitOfWork.Commit();

        _logger.LogInformation("Product created with ID {ProductId}", product.Id);

        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CategoryId,
            product.Images.Select(i => i.ImageUrl).ToList());
    }

    public async Task UpdateAsync(int id, UpdateProductRequest request)
    {
        _logger.LogInformation("Updating product {ProductId}: new name {Name}, new price {Price}", id, request.Name, request.Price);

        var product = await _productRepo.GetByIdAsync(id);
        if (product == null)
            throw new NotFoundException($"Product with id {id} not found.");

        // Verify category exists
        bool categoryExists = await _categoryRepo.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new NotFoundException($"Category with id {request.CategoryId} does not exist.");

        // Update properties
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.CategoryId = request.CategoryId;

        await _unitOfWork.Commit();
        _logger.LogInformation("Product {ProductId} updated", id);
    }

    public async Task AddImageAsync(int productId, Stream imageStream, string fileName)
    {
        _logger.LogInformation("Adding image to product {ProductId}", productId);

        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
            throw new NotFoundException($"Product with id {productId} not found.");

        var imageUrl = await _imageStorage.UploadAsync(imageStream, fileName);
        product.Images.Add(new ProductImage
        {
            ProductId = productId,
            ImageUrl = imageUrl
        });

        await _unitOfWork.Commit();
        _logger.LogInformation("Image added to product {ProductId}: {ImageUrl}", productId, imageUrl);
    }

    public async Task RemoveImageAsync(int productId, int imageId)
    {
        _logger.LogInformation("Removing image {ImageId} from product {ProductId}", imageId, productId);

        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
            throw new NotFoundException($"Product with id {productId} not found.");

        var image = product.Images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
            throw new NotFoundException($"Image with id {imageId} not found for product {productId}.");

        await _imageStorage.DeleteAsync(image.ImageUrl);
        product.Images.Remove(image);

        await _unitOfWork.Commit();
        _logger.LogInformation("Image {ImageId} removed from product {ProductId}", imageId, productId);
    }

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null)
            throw new NotFoundException($"Product with id {id} not found.");

        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CategoryId,
            product.Images.Select(i => i.ImageUrl).ToList());
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepo.GetAllAsync();
        _logger.LogInformation("Retrieved {Count} products", products.Count());
        return products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.Stock,
            p.CategoryId,
            p.Images.Select(i => i.ImageUrl).ToList()));
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting product {ProductId}", id);

        var product = await _productRepo.GetByIdAsync(id);
        if (product == null)
            throw new NotFoundException($"Product with id {id} not found.");

        // Delete associated images from storage
        foreach (var image in product.Images.ToList())
        {
            await _imageStorage.DeleteAsync(image.ImageUrl);
        }

        _productRepo.Remove(product);
        await _unitOfWork.Commit();

        _logger.LogInformation("Product {ProductId} deleted", id);
    }
}