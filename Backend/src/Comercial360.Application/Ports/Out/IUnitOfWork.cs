namespace Comercial360.Application.Ports.Out;

public interface IUnitOfWork
{
    Task<IUnitOfWorkTransaction> BeginSerializableTransactionAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IUnitOfWorkTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken);
}
