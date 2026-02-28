namespace Application.Abstraction;

public interface IUnitOfWork
{
    Task StartTransaction(CancellationToken cancellationToken = default);
    Task Rollback(CancellationToken cancellationToken = default);
    Task CommitTransaction(CancellationToken cancellationToken = default);
    Task<int> Commit();
}
