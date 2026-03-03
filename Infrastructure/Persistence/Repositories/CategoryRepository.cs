using Application.Abstraction;
using Domain.Entities;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Persistence.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<Category> CreateAsync(Category category)
    {
        await AddAsync(category);
        return category;
    }
    
    Task<IEnumerable<Category>> ICategoryRepository.GetAllAsync()
    {
        throw new NotImplementedException();
    }

    Task<Category?> ICategoryRepository.GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    void ICategoryRepository.Remove(Category category)
    {
        throw new NotImplementedException();
    }

    void ICategoryRepository.Update(Category category)
    {
        throw new NotImplementedException();
    }
}
