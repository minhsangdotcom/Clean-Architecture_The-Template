using Api.Middlewares;

namespace Api.Extensions;

public static class MiddlewareRegisterExtension
{
    public static void ExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionHandler>();
    }

    public static void CurrentUser(this IApplicationBuilder app)
    {
        app.UseMiddleware<UserMiddleware>();
    }

    public static void LogContext(this IApplicationBuilder app)
    {
        app.UseMiddleware<LogContextMiddleware>();
    }
}
