using System.Collections;
using System.Data.Common;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using Domain.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories;

public class UnitOfWork(IMapper mapper, IDbContext dbContext) : IUnitOfWork
{
    private Hashtable repositories = null!;

    private readonly IMapper mapper = mapper;

    private bool disposed = false;

    public DbConnection? Connection { get; set; } = null;

    public DbTransaction? Transaction { get; set; } = null;

    public async Task<DbTransaction> CreateTransactionAsync()
    {
        if (Connection != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        IDbContextTransaction currentTransaction =
            await dbContext.DatabaseFacade.BeginTransactionAsync();

        Transaction = currentTransaction.GetDbTransaction();
        Connection = currentTransaction.GetDbTransaction().Connection;

        return currentTransaction.GetDbTransaction();
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
            throw new InvalidOperationException("No transaction started.");
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

    public IRepository<TEntity> Repository<TEntity>()
        where TEntity : BaseEntity
    {
        repositories ??= [];

        var type = typeof(TEntity).Name;

        if (!repositories.ContainsKey(type))
        {
            var repositoryType = typeof(Repository<>);

            List<object> parameters = [dbContext, mapper];

            var repositoryInstance = Activator.CreateInstance(
                repositoryType.MakeGenericType(typeof(TEntity)),
                [.. parameters]
            );

            repositories.Add(type, repositoryInstance);
        }

        return (IRepository<TEntity>)repositories[type]!;
    }

    public int ExecuteSqlCommand(string sql, params object[] parameters) =>
        dbContext.DatabaseFacade.ExecuteSqlRaw(sql, parameters);

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
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

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            repositories?.Clear();
            dbContext.Dispose();
        }

        disposed = true;
    }
}
