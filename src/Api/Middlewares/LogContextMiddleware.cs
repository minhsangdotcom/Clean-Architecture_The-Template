using System.Diagnostics;

namespace Api.Middlewares;

public class LogContextMiddleware(RequestDelegate next, ILogger<LogContextMiddleware> logger)
{

    public Task InvokeAsync(HttpContext context)
    {
        var traceId = Activity.Current?.TraceId.ToString();
        var spanId = Activity.Current?.SpanId.ToString();

        using (logger.BeginScope("{@TraceId}", traceId))
        {
            context.Response.Headers.Append("trace-id", traceId);
            context.Response.Headers.Append("span-id", spanId);
            return next(context);
        }
    }
}
