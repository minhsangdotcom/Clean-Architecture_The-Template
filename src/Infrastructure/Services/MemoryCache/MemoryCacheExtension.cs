using Application.Common.Interfaces.Services.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.MemoryCache;

public static class MemoryCacheExtension
{
    public static IServiceCollection AddMemoryCaching(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .AddMemoryCache()
            .Configure<CacheSettings>(options =>
                configuration.GetSection(nameof(CacheSettings)).Bind(options)
            )
            .AddSingleton<IMemoryCacheService, MemoryCacheService>();
    }
}
