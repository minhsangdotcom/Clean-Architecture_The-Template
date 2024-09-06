using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces.Registers;
using Contracts.Extensions.Registers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDetection();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        return services.AddScoped<IDbContext, TheDbContext>()
                        .AddTransient<IUnitOfWork, UnitOfWork>()
                        .AddSingleton<UpdateBaseEntityInterceptor>()
                        .AddDbContext<TheDbContext>(
                            (sp, options) =>
                                options.UseNpgsql(configuration.GetConnectionString("default"))
                                    .AddInterceptors(sp.GetRequiredService<UpdateBaseEntityInterceptor>())
                        )
                        .AddAmazonS3(configuration)
                        .AddSingleton<ICurrentUser, CurrentUserService>()
                        .AddScoped(typeof(IAvatarUpdateService<>), typeof(AvatarUpdateService<>))
                        .Scan(scan => scan.FromCallingAssembly()
                            .AddClasses(classes => classes.AssignableTo<IScope>())
                                .AsImplementedInterfaces()
                                .WithScopedLifetime()
                            .AddClasses(classes => classes.AssignableTo<ISingleton>())
                                .AsImplementedInterfaces()
                                .WithSingletonLifetime()
                            .AddClasses(classes => classes.AssignableTo<ITransient>())
                                .AsImplementedInterfaces()
                                .WithTransientLifetime()
                        ).AddSingleton<IActionContextAccessor, ActionContextAccessor>();
    }
}