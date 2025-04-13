using Asp.Versioning;
using Asp.Versioning.Builder;

namespace Api.Extensions;

public static class ApiVersioningExtension
{
    public static IServiceCollection AddApiVersion(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version")
                );
            })
            .AddApiExplorer(config =>
            {
                config.GroupNameFormat = "'v'V";
                config.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}
