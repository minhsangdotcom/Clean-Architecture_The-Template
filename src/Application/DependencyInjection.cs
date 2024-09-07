using System.Reflection;
using Application.Common.Auth;
using Application.Common.Behaviors;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        Assembly currentAssembly = Assembly.GetExecutingAssembly();

        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
        
        return services
            .AddMediator(options => options.ServiceLifetime = ServiceLifetime.Transient)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformaceBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ProcessImagePathBehavior<,>))
            .AddValidatorsFromAssembly(currentAssembly)
            .AddAutoMapper(currentAssembly)
            .AddSingleton<IAuthorizationPolicyProvider, AuthorizePolicyProvider>()
            .AddSingleton<IAuthorizationHandler, AuthorizeHandler>();
    }
}
