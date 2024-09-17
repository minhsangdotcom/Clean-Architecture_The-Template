using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Data;

public interface IDbContext : IDisposable
{
    EntityEntry Entry(object entity);
    
    public DbSet<TEntity> Set<TEntity>() where TEntity : class;

    public DatabaseFacade DatabaseFacade { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    public Task UseTransactionAsync(DbTransaction transaction);
}