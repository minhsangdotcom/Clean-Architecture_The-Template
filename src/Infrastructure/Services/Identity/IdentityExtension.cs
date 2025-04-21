using Application.Common.Interfaces.Services.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Identity;

public static class IdentityExtension
{
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        return services
            .AddScoped<IRoleManagerService, RoleManagerService>()
            .AddScoped<IUserManagerService, UserManagerService>()
            .AddSingleton(typeof(IMediaUpdateService<>), typeof(MediaUpdateService<>));
    }
}
