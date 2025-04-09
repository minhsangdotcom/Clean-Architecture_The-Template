using System.Diagnostics;
using Api.Middlewares;
using Microsoft.AspNetCore.Http.Features;

namespace Api.Extensions;

public static class ProblemDetailExtention
{
    public static IServiceCollection AddErrorDetails(this IServiceCollection services)
    {
        return services
            .AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Instance =
                        $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                    context.ProblemDetails.Extensions.TryAdd(
                        "requestId",
                        context.HttpContext.TraceIdentifier
                    );
                };
            })
            .AddExceptionHandler<GlobalProblemDetailHandler>();
    }
}
