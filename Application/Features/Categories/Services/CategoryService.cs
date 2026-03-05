using Application.Abstraction;
using Application.Features.Categories.Dtos;
using Domain.Entities;

namespace Application.Features.Categories.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ICategoryRepository categoryRepo, IUnitOfWork unitOfWork)
    {
        _categoryRepo = categoryRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
    {
        // Use AnyAsync (inherited from generic repository)
        bool exists = await _categoryRepo.AnyAsync(c => c.Name == request.Name);
        if (exists)
            throw new InvalidOperationException("Category already exists.");

        var category = new Category { Name = request.Name };

        // Use AddAsync (inherited)
        await _categoryRepo.AddAsync(category);

        // Commit the transaction – this saves changes to the database
        await _unitOfWork.Commit();

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
}

