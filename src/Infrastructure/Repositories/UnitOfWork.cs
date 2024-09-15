using System.Collections;
using System.Data.Common;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using Contracts.Common;
using Domain.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace Infrastructure.Repositories;

public class UnitOfWork(IMapper mapper, IDbContext dbContext, ILogger logger) : IUnitOfWork
{
    private Hashtable repositories = null!;
    private bool disposed = false;

    public DbConnection? Connection { get; set; } = null;
    public DbTransaction? Transaction { get; set; } = null;

    public IRepository<TEntity> Repository<TEntity>()
        where TEntity : BaseEntity
    {
        repositories ??= [];
        var type = typeof(TEntity).Name;

        if (!repositories.ContainsKey(type))
        {
            var repositoryType = typeof(Repository<>);
            var repositoryInstance = Activator.CreateInstance(
                repositoryType.MakeGenericType(typeof(TEntity)),
                [dbContext, mapper]
            );

            repositories.Add(type, repositoryInstance);
        }

        return (IRepository<TEntity>)repositories[type]!;
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
        Connection = Transaction.Connection;

        return Transaction;
    }

    public async Task UseTransactionAsync(SharedTransaction transaction)
    {
        if (Transaction != null)
        {
            Connection = null;
            Transaction = null;
        }
        await dbContext.UseTransactionAsync(transaction.Transaction, transaction.Connection);

        Connection = transaction.Connection;
        Transaction = transaction.Transaction;
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
            Connection = null;
        }
    }
}
