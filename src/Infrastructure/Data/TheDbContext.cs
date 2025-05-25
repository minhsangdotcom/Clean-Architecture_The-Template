using System.Reflection;
using Application.Common.Interfaces.UnitOfWorks;
using Domain.Common;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Data;

public class TheDbContext(DbContextOptions<TheDbContext> options) : DbContext(options), IDbContext
{
    public DatabaseFacade DatabaseFacade => Database;

    public async Task DeleteRangeAsync<T>(IEnumerable<T> entities, Action<BulkConfig>? bulkConfig = null,
        CancellationToken cancellation = default) where T : class
    {
        BulkConfig defaultConfig = GetDefaultConfig();
        bulkConfig?.Invoke(defaultConfig);
        await this.BulkDeleteAsync(entities, defaultConfig, cancellationToken: cancellation);
    }

    public async Task InsertRangeAsync<T>(IEnumerable<T> entities,  Action<BulkConfig>? bulkConfig = null, CancellationToken cancellation = default) where T : class
    {
        BulkConfig defaultConfig = GetDefaultConfig();
        bulkConfig?.Invoke(defaultConfig);
        await this.BulkInsertAsync(entities, defaultConfig, cancellationToken: cancellation);
    }

    public async Task UpdateRangeAsync<T>(IEnumerable<T> entities, Action<BulkConfig>? bulkConfig = null,
        CancellationToken cancellation = default) where T : class
    {
        BulkConfig defaultConfig = GetDefaultConfig();
        bulkConfig?.Invoke(defaultConfig);
        await this.BulkUpdateAsync(entities, defaultConfig, cancellationToken: cancellation);
    }

    public async Task SynchronizeAsync<T>(IEnumerable<T> entities, Action<BulkConfig>? bulkConfig = null, CancellationToken cancellation = default) where T : class
    {
        BulkConfig defaultConfig = GetDefaultConfig();
        bulkConfig?.Invoke(defaultConfig);
        await this.BulkInsertOrUpdateOrDeleteAsync(entities, defaultConfig, cancellationToken: cancellation);
    }

    private static BulkConfig GetDefaultConfig() => new BulkConfig()
        { BatchSize = 1000, PreserveInsertOrder = false, BulkCopyTimeout = 60, SqlBulkCopyOptions = SqlBulkCopyOptions.KeepIdentity};

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