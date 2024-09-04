using System.Reflection;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Data;

public class TheDbContext(DbContextOptions<TheDbContext> options) :
    DbContext(options), IDbContext
{
    public DatabaseFacade DatabaseFacade => Database;

    public override DbSet<TEntity> Set<TEntity>() where TEntity : class => base.Set<TEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.HasPostgresExtension("citext");
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<Ulid>()
            .HaveConversion<UlidToStringConverter>();
    }
}