using System.Data.Common;
using Application.Common.Interfaces.UnitOfWorks;
using Infrastructure.UnitOfWorks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Infrastructure.UnitOfWorks;

public class UnitOfWork(
    IDbContext dbContext,
    ILogger logger,
    IServiceScopeFactory serviceScopeFactory
) : IUnitOfWork
{
    public DbTransaction? CurrentTransaction { get; set; }
    private bool disposed = false;

    public ISpecificationRepository<TEntity> SpecificationRepository<TEntity>(bool isCached = false)
        where TEntity : class
    {
        using IServiceScope serviceScope = serviceScopeFactory.CreateScope();
        IServiceProvider serviceProvider = serviceScope.ServiceProvider;

        if (isCached)
        {
            return serviceProvider.GetRequiredService<ISpecificationRepository<TEntity>>();
        }
        return serviceProvider.GetRequiredService<SpecificationRepository<TEntity>>();
    }

    public IStaticPredicateSpecificationRepository<TEntity> PredicateSpecificationRepository<TEntity>(
        bool isCached = false
    )
        where TEntity : class
    {
        using IServiceScope serviceScope = serviceScopeFactory.CreateScope();
        IServiceProvider serviceProvider = serviceScope.ServiceProvider;

        if (isCached)
        {
            return serviceProvider.GetRequiredService<
                IStaticPredicateSpecificationRepository<TEntity>
            >();
        }
        return serviceProvider.GetRequiredService<
            StaticPredicateSpecificationRepository<TEntity>
        >();
    }

    public async Task<DbTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default
    )
    {
        if (CurrentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        IDbContextTransaction currentTransaction =
            await dbContext.DatabaseFacade.BeginTransactionAsync(cancellationToken);

        CurrentTransaction = currentTransaction.GetDbTransaction();
        return CurrentTransaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (CurrentTransaction == null)
        {
            throw new InvalidOperationException("No transaction started.");
        }

        try
        {
            await CurrentTransaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await RollbackAsync(cancellationToken);
            throw new Exception("Transaction commit failed. Rolled back.", ex);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (CurrentTransaction == null)
        {
            logger.Warning("Thre is no transaction started.");
            return;
        }

        try
        {
            await CurrentTransaction.RollbackAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception("Transaction rollback failed.", ex);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public int ExecuteSqlCommand(string sql, params object[] parameters) =>
        dbContext.DatabaseFacade.ExecuteSqlRaw(sql, parameters);

    public async Task SaveAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            dbContext.Dispose();
        }

        disposed = true;
    }

    private async Task DisposeTransactionAsync()
    {
        if (CurrentTransaction != null)
        {
            await CurrentTransaction.DisposeAsync();
            CurrentTransaction = null;
        }
    }
}
