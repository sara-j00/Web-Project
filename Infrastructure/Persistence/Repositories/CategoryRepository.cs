using Application.Abstraction;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<bool> HasProductsAsync(int categoryId)
    {
        return await _context.Set<Product>().AnyAsync(p => p.CategoryId == categoryId);
    }
}
