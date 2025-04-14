using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.PostgreSql;
using HangfireBasicAuthenticationFilter;
using Infrastructure.Services.Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Hangfire;

public static class HangfireExtension
{
    public static IServiceCollection AddHangfireConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        if (configuration.GetValue<bool>("HangfireSettings:Enable"))
        {
            services.AddHangfireServer(options =>
                configuration.GetSection("HangfireSettings:Server").Bind(options)
            );

            services.AddHangfireConsoleExtensions();

            services.Configure<HangfireStorageSettings>(
                configuration.GetSection("HangfireSettings:Storage")
            );
            services.TryAddSingleton<
                IValidateOptions<HangfireStorageSettings>,
                ValidateHangfireStorage
            >();

            services.AddHangfire(
                (provider, hangfireConfiguration) =>
                {
                    HangfireStorageSettings hangfireStorageSettings = provider
                        .GetRequiredService<IOptions<HangfireStorageSettings>>()
                        .Value;
                    hangfireConfiguration.UsePostgreSqlStorage(
                        options =>
                        {
                            options.UseNpgsqlConnection(hangfireStorageSettings.ConnectionString);
                        },
                        configuration
                            .GetSection("HangfireSettings:Storage:Options")
                            .Get<PostgreSqlStorageOptions>()
                    );
                    hangfireConfiguration.UseConsole();
                }
            );
        }

        return services;
    }

    public static IApplicationBuilder UseHangfireDashboard(
        this IApplicationBuilder app,
        IConfiguration configuration
    )
    {
        if (configuration.GetValue<bool>("HangfireSettings:Enable"))
        {
            DashboardOptions dashboardOptions =
                configuration.GetSection("HangfireSettings:Dashboard").Get<DashboardOptions>()
                ?? new();
            dashboardOptions.Authorization =
            [
                new HangfireCustomBasicAuthenticationFilter
                {
                    User = configuration["HangfireSettings:Credentials:Username"],
                    Pass = configuration["HangfireSettings:Credentials:Password"],
                },
            ];
            app.UseHangfireDashboard(configuration["HangfireSettings:Route"], dashboardOptions);
        }

        return app;
    }
}
