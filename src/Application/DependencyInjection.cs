using System.Reflection;
using Application.Common.Auth;
using Application.Common.Behaviors;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        Assembly currentAssembly = Assembly.GetExecutingAssembly();

        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

        services
            .AddMediator(options => options.ServiceLifetime = ServiceLifetime.Singleton);

         return services
            .AddSingleton(typeof(IPipelineBehavior<,>), typeof(ErrorLoggingBehaviour<,>))
            .AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddSingleton(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddSingleton(typeof(IPipelineBehavior<,>), typeof(PerformaceBehavior<,>))
            .AddSingleton(typeof(IPipelineBehavior<,>), typeof(ProcessImagePathBehavior<,>))
            .AddValidatorsFromAssembly(currentAssembly)
            .AddAutoMapper(currentAssembly)
            .AddSingleton<IAuthorizationPolicyProvider, AuthorizePolicyProvider>()
            .AddSingleton<IAuthorizationHandler, AuthorizeHandler>();
    }
}
