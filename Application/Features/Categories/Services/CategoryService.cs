using Application.Abstraction;
using Application.Exceptions; // for NotFoundException, ConflictException
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

        bool exists = await _categoryRepo.AnyAsync(c => c.Name == request.Name);
        if (exists)
        {
            _logger.LogWarning("Category creation failed: name {CategoryName} already exists", request.Name);
            throw new ConflictException("Category already exists."); // 409
        }

        var category = new Category { Name = request.Name };
        await _categoryRepo.AddAsync(category);
        await _unitOfWork.Commit();

        _logger.LogInformation("Category created with ID {CategoryId}, name {CategoryName}", category.Id, category.Name);
        return new CategoryDto(category.Id, category.Name);
    }

    public async Task UpdateAsync(int id, UpdateCategoryRequest request)
    {
        _logger.LogInformation("Attempting to update category {CategoryId}", id);

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Category name is required.");

        var category = await _categoryRepo.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("Update failed: category {CategoryId} not found", id);
            throw new NotFoundException($"Category with id {id} not found."); // 404
        }

        bool nameExists = await _categoryRepo.AnyAsync(c => c.Name == request.Name && c.Id != id);
        if (nameExists)
        {
            _logger.LogWarning("Update failed: name {CategoryName} already used by another category", request.Name);
            throw new ConflictException("A category with this name already exists."); // 409
        }

        string oldName = category.Name;
        category.Name = request.Name;
        await _unitOfWork.Commit();

        _logger.LogInformation("Category {CategoryId} updated: name changed from '{OldName}' to '{NewName}'", id, oldName, request.Name);
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Attempting to delete category {CategoryId}", id);

        var category = await _categoryRepo.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("Delete failed: category {CategoryId} not found", id);
            throw new NotFoundException($"Category with id {id} not found."); // 404
        }

        bool hasProducts = await _categoryRepo.HasProductsAsync(id);
        if (hasProducts)
        {
            _logger.LogWarning("Delete failed: category {CategoryId} has associated products", id);
            throw new InvalidOperationException("Cannot delete category that has products."); // 400 (or could be 409)
        }

        _categoryRepo.Remove(category);
        await _unitOfWork.Commit();

        _logger.LogInformation("Category {CategoryId} deleted", id);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepo.GetAll();
        _logger.LogInformation("Retrieved {Count} categories", categories.Count());
        return categories.Select(c => new CategoryDto(c.Id, c.Name));
    }

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        var category = await _categoryRepo.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("Category {CategoryId} not found", id);
            throw new NotFoundException($"Category with id {id} not found."); // 404
        }
        return new CategoryDto(category.Id, category.Name);
    }
}