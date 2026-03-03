using Domain.Entities;

namespace Application.Abstraction;

public interface IProductRepository
{
    Task<Product> CreateAsync(Product product);
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    void Update(Product product);
    void Remove(Product product);

}
