using Application.Abstraction;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<Product> CreateAsync(Product product)
    {
        await AddAsync(product);
        return product;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.Images)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    void IProductRepository.Remove(Product product)
    {
        throw new NotImplementedException();
    }

    void IProductRepository.Update(Product product)
    {
        throw new NotImplementedException();
    }
}
