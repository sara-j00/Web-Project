using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Application.Abstraction;

namespace Infrastructure.Persistence.UnitOfWork;


public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;
    public async Task StartTransaction(CancellationToken cancellationToken = default)
    {
        _transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task Rollback(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task CommitTransaction(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task<int> Commit()
    {
        return await dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        dbContext.Dispose();
    }
}
