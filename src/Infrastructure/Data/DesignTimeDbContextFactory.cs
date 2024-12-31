using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TheDbContext>
{
    public TheDbContext CreateDbContext(string[] args)
    {
        string environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                @Directory.GetCurrentDirectory() + $"/../Api/appsettings.{environment}.json"
            )
            .Build();
        string? connectionString = configuration.GetValue<string>(
            "DatabaseSettings:DatabaseConnection"
        );
        var builder = new DbContextOptionsBuilder<TheDbContext>();
        builder.UseNpgsql(connectionString);
        return new TheDbContext(builder.Options);
    }
}
