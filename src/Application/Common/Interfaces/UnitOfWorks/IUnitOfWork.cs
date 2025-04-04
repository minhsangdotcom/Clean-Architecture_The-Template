using System.Data.Common;

namespace Application.Common.Interfaces.UnitOfWorks;

public interface IUnitOfWork : IDisposable
{
    public DbTransaction? CurrentTransaction { get; protected set; }

    ISpecificationRepository<TEntity> SpecificationRepository<TEntity>(bool isCached = false)
        where TEntity : class;

    IStaticPredicateSpecificationRepository<TEntity> PredicateSpecificationRepository<TEntity>(
        bool isCached = false
    )
        where TEntity : class;

    Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);

    int ExecuteSqlCommand(string sql, params object[] parameters);

    Task SaveAsync(CancellationToken cancellationToken = default);
}
