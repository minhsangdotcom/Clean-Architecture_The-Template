using System.Data.Common;

namespace Application.Common.Interfaces.UnitOfWorks;

public interface IUnitOfWork : IDisposable
{
    public DbTransaction? CurrentTransaction { get; protected set; }

    IAsyncRepository<TEntity> Repository<TEntity>(bool isCached = false)
        where TEntity : class;

    /// <summary>
    /// Read-only operations in specification pattern
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="isCached">Do cache or not</param>
    /// <returns></returns>
    IDynamicSpecificationRepository<TEntity> ReadOnlyRepository<TEntity>(bool isCached = false)
        where TEntity : class;

    /// <summary>
    /// Read-only operations in specification pattern by static query
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="isCached">Do cache or not</param>
    /// <returns></returns>
    ISpecificationRepository<TEntity> StaticReadOnlyRepository<TEntity>(
        bool isCached = false
    )
        where TEntity : class;

    Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);

    int ExecuteSqlCommand(string sql, params object[] parameters);

    Task SaveAsync(CancellationToken cancellationToken = default);
}
