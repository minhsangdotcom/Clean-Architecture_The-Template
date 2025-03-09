using System.Data.Common;
using Application.Common.Interfaces.Services.Cache;
using Application.Common.Interfaces.UnitOfWorks;
using AutoMapper;
using Infrastructure.UnitOfWorks.CachedRepositories;
using Infrastructure.UnitOfWorks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using SharedKernel.Common;

namespace Infrastructure.UnitOfWorks;

public class UnitOfWork(
    IMapper mapper,
    IDbContext dbContext,
    ILogger logger,
    IMemoryCacheService memoryCacheService
) : IUnitOfWork
{
    public DbTransaction? CurrentTransaction { get; set; }

    private readonly Dictionary<string, object?> repositories = [];
    private bool disposed = false;

    public IRepository<TEntity> Repository<TEntity>()
        where TEntity : class
    {
        typeof(TEntity).IsValidBaseType();
        string type = typeof(TEntity).FullName!;

        if (!repositories.TryGetValue(type, out object? value))
        {
            Type repositoryType = typeof(Repository<>);
            object? repositoryInstance = Activator.CreateInstance(
                repositoryType.MakeGenericType(typeof(TEntity)),
                [dbContext, mapper]
            );
            value = repositoryInstance;
            repositories.Add(type, value);
        }

        return (IRepository<TEntity>)value!;
    }

    public IRepository<TEntity> CachedRepository<TEntity>()
        where TEntity : class
    {
        typeof(TEntity).IsValidBaseType();
        string type = $"{typeof(TEntity).FullName}-cached";

        if (!repositories.TryGetValue(type, out object? value))
        {
            Type cachedRepositoryType = typeof(CachedRepository<>);
            Type repositoryType = typeof(Repository<>);

            object? repositoryInstance = Activator.CreateInstance(
                repositoryType.MakeGenericType(typeof(TEntity)),
                [dbContext, mapper]
            );
            // proxy design pattern
            object? cachedRepositoryInstance = Activator.CreateInstance(
                cachedRepositoryType.MakeGenericType(typeof(TEntity)),
                [repositoryInstance, logger, memoryCacheService]
            );
            value = cachedRepositoryInstance;
            repositories.Add(type, value);
        }

        return (IRepository<TEntity>)value!;
    }

    public async Task<DbTransaction> CreateTransactionAsync(
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
            repositories?.Clear();
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
