using Application.Features.Categories.Dtos;

namespace Application.Features.Categories.Services;

public interface ICategoryService
{
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request);
    Task UpdateAsync(int id, UpdateCategoryRequest request);
    Task DeleteAsync(int id);
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto> GetByIdAsync(int id);
}
