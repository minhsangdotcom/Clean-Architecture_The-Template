using System.Data.Common;
using Domain.Aggregates.Users;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Respawn;

namespace Application.SubcutaneousTests;

public class PostgreSqlDatabase : IDatabase
{
    private NpgsqlConnection? connection;

    private readonly string? connectionString;
    private Respawner? respawner;

    public PostgreSqlDatabase()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Testing.json")
            .AddEnvironmentVariables()
            .Build();

        connectionString = configuration["DatabaseSettings:DatabaseConnection"];
    }

    public async Task InitialiseAsync()
    {
        connection = new NpgsqlConnection(connectionString);

        var options = new DbContextOptionsBuilder<TheDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        var context = new TheDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.Migrate();

        await connection.OpenAsync();
        respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                // don't remove these tables
                TablesToIgnore = ["__EFMigrationsHistory", "province", "district", "commune"],
            }
        );
        await connection.CloseAsync();
    }

    public DbConnection GetConnection() => connection!;

    public string GetConnectionString() => connectionString!;

    public async Task ResetAsync()
    {
        if (respawner != null && connection != null)
        {
            await connection.OpenAsync();
            await respawner.ResetAsync(connection);
            await connection.CloseAsync();
        }
    }

    public async Task DisposeAsync()
    {
        if (connection != null)
        {
            await connection.CloseAsync();
            await connection.DisposeAsync();
        }
    }
}
