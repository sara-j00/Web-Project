using Application.Features.Categories.Dtos;

namespace Application.Features.Categories.Services;

public interface ICategoryService
{
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request);
    Task UpdateAsync(int id, UpdateCategoryRequest request);
}
