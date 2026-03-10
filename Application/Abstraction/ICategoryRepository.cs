using Domain.Entities;

namespace Application.Abstraction;

public interface ICategoryRepository : IGenericRepository<Category> 
{
    Task<bool> HasProductsAsync(int categoryId);
}
