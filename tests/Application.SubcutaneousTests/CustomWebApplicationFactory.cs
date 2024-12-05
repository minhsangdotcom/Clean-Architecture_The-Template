using System.Data;
using System.Data.Common;
using Infrastructure.Data;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace Application.SubcutaneousTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        try
        {
            builder.UseEnvironment("Testing");
            var host = builder.Build();
            host.Start();

            using var scope = host.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var db = scopedServices.GetRequiredService<TheDbContext>();
            // var logger = scopedServices.GetRequiredService<
            //     ILogger<CustomWebApplicationFactory<TProgram>>
            // >();
            // Reset database for a clean slate for testing
            //db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            return host;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred initializing the database. Error: {ex.Message}");
            throw; // Rethrow to avoid silently swallowing exceptions
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<TheDbContext>)
            );

            services.Remove(descriptor!);

            var dbConnectionDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbConnection)
            );

            services.Remove(dbConnectionDescriptor!);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Testing.json")
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration["DatabaseSettings:DatabaseConnection"];

            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new NpgsqlConnection(connectionString);

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                return connection;
            });

            services.AddDbContext<TheDbContext>(
                (container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseNpgsql(connection);
                }
            );
        });
        builder.UseEnvironment("Testing");
    }
}
