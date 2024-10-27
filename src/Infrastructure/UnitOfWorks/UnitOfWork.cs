using System.Data.Common;
using Application.Common.Interfaces.UnitOfWorks;
using AutoMapper;
using Domain.Common;
using Infrastructure.Data;
using Infrastructure.UnitOfWorks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace Infrastructure.UnitOfWorks;

public class UnitOfWork(IMapper mapper, IDbContext dbContext, ILogger logger) : IUnitOfWork
{
    public DbTransaction? Transaction { get; set; } =
        dbContext.DatabaseFacade.CurrentTransaction?.GetDbTransaction();
    private readonly Dictionary<string, object?> repositories = [];
    private bool disposed = false;

    public IRepository<TEntity> Repository<TEntity>()
        where TEntity : class
    {
        typeof(TEntity).IsValidBaseType();
        string type = typeof(TEntity).Name;

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

    public async Task<DbTransaction> CreateTransactionAsync()
    {
        if (Transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        IDbContextTransaction currentTransaction =
            await dbContext.DatabaseFacade.BeginTransactionAsync();

        Transaction = currentTransaction.GetDbTransaction();
        return Transaction;
    }

    public async Task UseTransactionAsync(DbTransaction transaction)
    {
        await dbContext.UseTransactionAsync(transaction);
        if (Transaction == null || Transaction != transaction)
        {
            Transaction = transaction;
        }
    }

    public async Task CommitAsync()
    {
        if (Transaction == null)
        {
            throw new InvalidOperationException("No transaction started.");
        }

        try
        {
            await Transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await RollbackAsync();
            throw new Exception("Transaction commit failed. Rolled back.", ex);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackAsync()
    {
        if (Transaction == null)
        {
            logger.Warning("Thre is no transaction started.");
            return;
        }

        try
        {
            await Transaction.RollbackAsync();
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
        if (Transaction != null)
        {
            await Transaction.DisposeAsync();
            Transaction = null;
        }
    }
}
