using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Common.Interfaces.UnitOfWorks;

public interface IDbContext : IDisposable
{
    EntityEntry Entry(object entity);

    public DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    public DatabaseFacade DatabaseFacade { get; }

    public Task DeleteRangeAsync<T>(IEnumerable<T> entities, Action<BulkConfig>? bulkConfig = null,
        CancellationToken cancellation = default) where T : class;

    public Task InsertRangeAsync<T>(IEnumerable<T> entities, Action<BulkConfig>? bulkConfig = null,
        CancellationToken cancellation = default) where T : class;

    public Task UpdateRangeAsync<T>(IEnumerable<T> entities, Action<BulkConfig>? bulkConfig = null,
        CancellationToken cancellation = default) where T : class;

    public Task SynchronizeAsync<T>(IEnumerable<T> entities, Action<BulkConfig>? bulkConfig = null,
        CancellationToken cancellation = default) where T : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}