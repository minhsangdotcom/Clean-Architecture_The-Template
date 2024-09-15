using System.Data;
using System.Data.Common;
using System.Reflection;
using Ardalis.GuardClauses;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Serilog;

namespace Infrastructure.Data;

public class TheDbContext(DbContextOptions<TheDbContext> options, ILogger logger) : DbContext(options), IDbContext
{
    private bool IsSharedTransaction = false;
    public DatabaseFacade DatabaseFacade => Database;

    public override DbSet<TEntity> Set<TEntity>()
        where TEntity : class => base.Set<TEntity>();

    public async Task UseTransactionAsync(
        DbTransaction transaction,
        DbConnection? connection = null
    )
    {
        var dbConnection = Database.GetDbConnection();

        if (connection != null && dbConnection != connection)
        {
            Database.SetDbConnection(connection);
        }

        if (dbConnection.State == ConnectionState.Closed)
        {
            dbConnection.Open();
        }

        Guard.Against.Null(transaction, nameof(transaction), "transaction is not null");

        await Database.UseTransactionAsync(transaction);
        IsSharedTransaction = true;
    }

    public async Task CommitTransactionAsync()
    {
        if (IsSharedTransaction)
        {
            logger.Warning("there is no transaction to commit!");
            return;
        }
        await Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        if (IsSharedTransaction)
        {
            logger.Warning("there is no transaction to rollback!");
            return;
        }
        await Database.RollbackTransactionAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.HasPostgresExtension("citext");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Ulid>().HaveConversion<UlidToStringConverter>();
    }
}
