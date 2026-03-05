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
}

