using Application.Abstraction;
using Application.Features.Categories.Dtos;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository categoryRepo, IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
    {
        _categoryRepo = categoryRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
    {
        _logger.LogInformation("Attempting to create category with name {CategoryName}", request.Name);

        // Use AnyAsync (inherited from generic repository)
        bool exists = await _categoryRepo.AnyAsync(c => c.Name == request.Name);
        if (exists)
        {
            _logger.LogWarning("Category creation failed: name {CategoryName} already exists", request.Name);
            
            throw new InvalidOperationException("Category already exists.");
        }

        var category = new Category { Name = request.Name };

        // Use AddAsync (inherited)
        await _categoryRepo.AddAsync(category);

        // Commit the transaction – this saves changes to the database
        await _unitOfWork.Commit();

        _logger.LogInformation("Category created with ID {CategoryId}, name {CategoryName}", category.Id, category.Name);

        return new CategoryDto(category.Id, category.Name);
    }

    public async Task UpdateAsync(int id, UpdateCategoryRequest request)
{
    // Basic validation (until FluentValidation)
    if (string.IsNullOrWhiteSpace(request.Name))
        throw new ArgumentException("Category name is required.");

    var category = await _categoryRepo.GetByIdAsync(id);
    if (category == null)
        throw new InvalidOperationException($"Category with id {id} not found.");

    // Optional: prevent duplicate names (excluding itself)
    bool nameExists = await _categoryRepo.AnyAsync(c => c.Name == request.Name && c.Id != id);
    if (nameExists)
        throw new InvalidOperationException("A category with this name already exists.");

    category.Name = request.Name;

    // Entity is tracked – no need to call Update()
    await _unitOfWork.Commit();
}

    public async Task DeleteAsync(int id)
    {
        var category = await _categoryRepo.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("Delete failed: category {CategoryId} not found", id);

            throw new InvalidOperationException($"Category with id {id} not found.");
        }

        // Optional: prevent deletion if category has products
        // You need a way to check related products – either through a ProductRepository or navigation property
        // Example using a hypothetical method:
        bool hasProducts = await _categoryRepo.HasProductsAsync(id); // You'd need to implement this in ICategoryRepository
        if (hasProducts)
            throw new InvalidOperationException("Cannot delete category that has products.");

        _categoryRepo.Remove(category);
        await _unitOfWork.Commit();

        _logger.LogInformation("Category {CategoryId} deleted", id);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepo.GetAll();
        return categories.Select(c => new CategoryDto(c.Id, c.Name));
    }

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        var category = await _categoryRepo.GetByIdAsync(id);
        if (category == null)
            throw new InvalidOperationException($"Category with id {id} not found.");

        return new CategoryDto(category.Id, category.Name);
    }
}

