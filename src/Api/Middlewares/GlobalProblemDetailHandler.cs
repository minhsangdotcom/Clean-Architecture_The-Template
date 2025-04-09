using Application.Common.Errors;
using Contracts.ApiWrapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middlewares;

public class GlobalProblemDetailHandler(
    IProblemDetailsService problemDetailsService,
    Serilog.ILogger logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is not ErrorDetails errorDetails)
        {
            return true;
        }

        IHttpActivityFeature? activityFeature = httpContext.Features.Get<IHttpActivityFeature>();
        string? traceId = activityFeature?.Activity?.TraceId.ToString();
        string? spanId = activityFeature?.Activity?.SpanId.ToString();
        logger.Error(
            "\n\n Server {exception} error has {@trace}  error is with message '{Message}'\n {StackTrace}\n at {DatetimeUTC} \n",
            exception.GetType().Name,
            new TraceLogging() { TraceId = traceId, SpanId = spanId },
            exception.Message,
            exception.StackTrace?.TrimStart(),
            DateTimeOffset.UtcNow
        );

        int code = StatusCodes.Status500InternalServerError;
        httpContext.Response.StatusCode = code;

        ProblemDetails problemDetail =
            new()
            {
                Status = code,
                Title = errorDetails.Title,
                Detail = errorDetails.Message,
                Type = errorDetails.Type,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                Extensions = new Dictionary<string, object?>()
                {
                    { "traceId", traceId },
                    { "spanId", spanId },
                    { "requestId", httpContext.TraceIdentifier },
                },
            };

        if (
            !await problemDetailsService.TryWriteAsync(
                new()
                {
                    HttpContext = httpContext,
                    ProblemDetails = problemDetail,
                    Exception = exception,
                }
            )
        )
        {
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(
                problemDetail,
                typeof(ProblemDetails),
                cancellationToken
            );
        }

        return true;
    }
}
