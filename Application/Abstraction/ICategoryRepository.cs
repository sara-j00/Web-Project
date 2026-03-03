using Domain.Entities;

namespace Application.Abstraction;

public interface ICategoryRepository
{
    Task<Category> CreateAsync(Category category);
    Task<Category?> GetByIdAsync(Guid id);
    Task<IEnumerable<Category>> GetAllAsync();
    void Update(Category category);
    void Remove(Category category);
}
