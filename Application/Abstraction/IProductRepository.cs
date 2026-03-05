using Domain.Entities;

namespace Application.Abstraction;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
}
