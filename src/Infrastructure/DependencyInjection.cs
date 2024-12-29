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
using Infrastructure.Services.Hangfires;
using Infrastructure.Services.Identity;
using Infrastructure.Services.Mail;
using Infrastructure.Services.Token;
using Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        string? environmentName = "Development"
    )
    {
        services.AddDetection();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.Configure<DatabaseSettings>(options =>
            configuration.GetSection(nameof(DatabaseSettings)).Bind(options)
        );
        services.TryAddSingleton<IValidateOptions<DatabaseSettings>, ValidateDatabaseSetting>();

        services.AddSingleton(sp =>
        {
            var databaseSettings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            string connectionString = databaseSettings.DatabaseConnection!;
            return new NpgsqlDataSourceBuilder(connectionString).EnableDynamicJson().Build();
        });

        services
            .AddScoped<IDbContext, TheDbContext>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddSingleton<UpdateAuditableEntityInterceptor>()
            .AddSingleton<DispatchDomainEventInterceptor>();

        if (environmentName!.CompareTo("Testing") == 0)
        {
            services.AddDbContext<TheDbContext>(
                (sp, options) =>
                {
                    NpgsqlDataSource npgsqlDataSource = sp.GetRequiredService<NpgsqlDataSource>();
                    options
                        .UseNpgsql(npgsqlDataSource)
                        .AddInterceptors(
                            sp.GetRequiredService<UpdateAuditableEntityInterceptor>(),
                            sp.GetRequiredService<DispatchDomainEventInterceptor>()
                        );
                }
            );
        }
        else
        {
            services.AddDbContextPool<TheDbContext>(
                (sp, options) =>
                {
                    NpgsqlDataSource npgsqlDataSource = sp.GetRequiredService<NpgsqlDataSource>();
                    options
                        .UseNpgsql(npgsqlDataSource)
                        .AddInterceptors(
                            sp.GetRequiredService<UpdateAuditableEntityInterceptor>(),
                            sp.GetRequiredService<DispatchDomainEventInterceptor>()
                        );
                }
            );
        }

        services
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
            .AddMemoryCache()
            .AddRedis(configuration)
            .AddHangfireConfiguration(configuration)
            .AddElasticSearch(configuration);

        return services;
    }
}
