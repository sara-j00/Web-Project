using Application.Abstraction;
using Domain.Entities;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Persistence.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

}
