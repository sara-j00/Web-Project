using Application.Features.Categories.Dtos;

namespace Application.Features.Categories.Services;

public interface ICategoryService
{
    Task<CreateCategoryRequest> CreateAsync(string name);
    Task<IEnumerable<CreateCategoryRequest>> GetAllAsync();
}
