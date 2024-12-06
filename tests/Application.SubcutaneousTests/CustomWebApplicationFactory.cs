using System.Data.Common;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.SubcutaneousTests;

public class CustomWebApplicationFactory<TProgram>(DbConnection dbConnection)
    : WebApplicationFactory<TProgram>
    where TProgram : class
{
    private const string EnvironmentName = "Testing";

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(EnvironmentName);
        IHost host = builder.Build();
        host.Start();

        using IServiceScope scope = host.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;
        TheDbContext db = scopedServices.GetRequiredService<TheDbContext>();
        var logger = scopedServices.GetRequiredService<
            ILogger<CustomWebApplicationFactory<TProgram>>
        >();
        try
        {
            db.Database.EnsureCreated();
            //do seeding database here

            return host;
        }
        catch (Exception ex)
        {
            logger.LogError(
                "An error occurred initializing the database. Error: {error}",
                ex.Message
            );
            throw;
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

            services.AddDbContext<TheDbContext>(
                (container, options) => options.UseNpgsql(dbConnection)
            );
        });
        builder.UseEnvironment(EnvironmentName);
    }
}
