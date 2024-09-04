using System.Collections;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using Domain.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UnitOfWork(IMapper mapper, IDbContext dbContext) : IUnitOfWork
{
    private Hashtable repositories = null!;

    private readonly IMapper mapper = mapper;

    private bool disposed = false;

    public async Task CreateTransactionAsync()
    {
        await dbContext.DatabaseFacade.BeginTransactionAsync();
    }

    public async Task RollbackAsync()
    {
        await dbContext.DatabaseFacade.RollbackTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await dbContext.DatabaseFacade.CommitTransactionAsync();
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        repositories ??= [];

        var type = typeof(TEntity).Name;

        if (!repositories.ContainsKey(type))
        {
            var repositoryType = typeof(Repository<>);

            List<object> parameters = [dbContext, mapper];

            var repositoryInstance =
                Activator.CreateInstance(
                    repositoryType.MakeGenericType(typeof(TEntity)),
                    [.. parameters]);

            repositories.Add(type, repositoryInstance);
        }

        return (IRepository<TEntity>)repositories[type]!;
    }

    public int ExecuteSqlCommand(string sql, params object[] parameters) =>
        dbContext.DatabaseFacade.ExecuteSqlRaw(sql, parameters);

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // clear repositories
                repositories?.Clear();

                // dispose the db context.
                dbContext.Dispose();
            }
        }

        disposed = true;
    }
}