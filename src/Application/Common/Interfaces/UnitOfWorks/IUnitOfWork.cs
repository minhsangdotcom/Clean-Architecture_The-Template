using System.Data.Common;
namespace Application.Common.Interfaces.UnitOfWorks;

public interface IUnitOfWork : IDisposable
{
    public DbTransaction? Transaction { get; protected set; }

    IRepository<TEntity> Repository<TEntity>()
        where TEntity : class;

    Task<DbTransaction> CreateTransactionAsync();

    Task UseTransactionAsync(DbTransaction transaction);

    Task CommitAsync();

    Task RollbackAsync();

    int ExecuteSqlCommand(string sql, params object[] parameters);

    Task SaveAsync(CancellationToken cancellationToken = default);
}
