using System.Data.Common;
using Application.Common.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Application.SubcutaneousTests;

public class CustomWebApplicationFactory<TProgram>(
    DbConnection dbConnection,
    string environmentName
) : WebApplicationFactory<TProgram>
    where TProgram : class
{
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

            services
                .RemoveAll<ICurrentUser>()
                .AddTransient(provider =>
                    Mock.Of<ICurrentUser>(x => x.Id == TestingFixture.GetUserId())
                );
        });
        builder.UseEnvironment(environmentName);
    }
}
