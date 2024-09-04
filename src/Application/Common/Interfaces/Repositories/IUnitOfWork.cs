using Domain.Common;

namespace Application.Common.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
        IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

        Task CreateTransactionAsync();

        Task CommitAsync();

        Task RollbackAsync();

        int ExecuteSqlCommand(string sql, params object[] parameters);

        Task SaveAsync(CancellationToken cancellationToken = default);
}