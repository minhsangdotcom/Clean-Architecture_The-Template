using Application.Common.Interfaces.Registers;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.Services.Mail;
using Application.Common.Interfaces.UnitOfWorks;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Services;
using Infrastructure.Services.Aws;
using Infrastructure.Services.DistributedCache;
using Infrastructure.Services.Elastics;
using Infrastructure.Services.Identity;
using Infrastructure.Services.Mail;
using Infrastructure.Services.Token;
using Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDetection();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services
            .AddScoped<IDbContext, TheDbContext>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddSingleton<UpdateAuditableEntityInterceptor>()
            .AddSingleton<DispatchDomainEventInterceptor>()
            .AddDbContext<TheDbContext>(
                (sp, options) =>
                    options
                        .UseNpgsql(
                            new NpgsqlDataSourceBuilder(
                                configuration.GetConnectionString("default")
                            )
                                .EnableDynamicJson()
                                .Build()
                        )
                        .AddInterceptors(
                            sp.GetRequiredService<UpdateAuditableEntityInterceptor>(),
                            sp.GetRequiredService<DispatchDomainEventInterceptor>()
                        )
            )
            .AddAmazonS3(configuration)
            .AddSingleton<ICurrentUser, CurrentUserService>()
            .AddSingleton(typeof(IMediaUpdateService<>), typeof(MediaUpdateService<>))
            .AddTransient<KitMailService>()
            .AddTransient<IMailService, KitMailService>(provider =>
                provider.GetService<KitMailService>()!
            )
            .AddTransient<FluentMailService>()
            .AddTransient<IMailService, FluentMailService>(provider =>
                provider.GetService<FluentMailService>()!
            )
            .AddFluentMail(configuration)
            .Scan(scan =>
                scan.FromCallingAssembly()
                    .AddClasses(classes => classes.AssignableTo<IScope>())
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                    .AddClasses(classes => classes.AssignableTo<ISingleton>())
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime()
                    .AddClasses(classes => classes.AssignableTo<ITransient>())
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
            )
            .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
            .AddJwtAuth(configuration)
            .AddElasticSearch(configuration)
            .AddHostedService<ElasticsearchIndexBackgoundService>()
            .AddMemoryCache()
            .AddRedis(configuration);

        return services;
    }
}
