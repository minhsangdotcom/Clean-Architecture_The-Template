using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TheDbContext>
{
    public TheDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(@Directory.GetCurrentDirectory() + "/../Api/appsettings.Development.json")
            .Build();
        var builder = new DbContextOptionsBuilder<TheDbContext>();
        var connectionString = configuration.GetConnectionString("default");
        builder.UseNpgsql(connectionString);
        return new TheDbContext(builder.Options, Log.Logger);
    }
}
