using System.Data.Common;

namespace Application.Common.Interfaces.UnitOfWorks;

public interface IUnitOfWork : IDisposable
{
    public DbTransaction? CurrentTransaction { get; protected set; }

    IRepository<TEntity> Repository<TEntity>()
        where TEntity : class;

    IRepository<TEntity> CachedRepository<TEntity>()
        where TEntity : class;

    Task<DbTransaction> CreateTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);

    int ExecuteSqlCommand(string sql, params object[] parameters);

    Task SaveAsync(CancellationToken cancellationToken = default);
}
