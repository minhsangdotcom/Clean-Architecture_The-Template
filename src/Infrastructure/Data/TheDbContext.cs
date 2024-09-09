using System.Data;
using System.Data.Common;
using System.Reflection;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Data;

public class TheDbContext(DbContextOptions<TheDbContext> options) : DbContext(options), IDbContext
{
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

        await Database.UseTransactionAsync(transaction);
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
