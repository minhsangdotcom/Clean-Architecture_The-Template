using System.Data;
using System.Data.Common;
using System.Reflection;
using Application.Common.Interfaces.UnitOfWorks;
using Ardalis.GuardClauses;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Data;

public class TheDbContext(DbContextOptions<TheDbContext> options) : DbContext(options), IDbContext
{
    public DatabaseFacade DatabaseFacade => Database;

    public override DbSet<TEntity> Set<TEntity>()
        where TEntity : class => base.Set<TEntity>();

    public async Task UseTransactionAsync(DbTransaction transaction)
    {
        DbConnection dbConnection = Database.GetDbConnection();

        if (dbConnection.State == ConnectionState.Closed)
        {
            dbConnection.Open();
        }

        Guard.Against.Null(transaction, nameof(transaction), "transaction is not null");
        await Database.UseTransactionAsync(transaction);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.HasPostgresExtension("citext");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSnakeCaseNamingConvention();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) =>
        configurationBuilder.Properties<Ulid>().HaveConversion<UlidToStringConverter>();
}
