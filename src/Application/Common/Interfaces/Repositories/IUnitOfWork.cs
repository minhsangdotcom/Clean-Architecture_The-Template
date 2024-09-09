using System.Data.Common;
using Domain.Common;

namespace Application.Common.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    public DbConnection? Connection { get; protected set; }

    public DbTransaction? Transaction { get; protected set; }

    IRepository<TEntity> Repository<TEntity>()
        where TEntity : BaseEntity;

    Task<DbTransaction> CreateTransactionAsync();

    Task CommitAsync();

    Task RollbackAsync();

    int ExecuteSqlCommand(string sql, params object[] parameters);

    Task SaveAsync(CancellationToken cancellationToken = default);
}
