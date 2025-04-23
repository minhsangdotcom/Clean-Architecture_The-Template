using Api.Middlewares;

namespace Api.Extensions;

public static class MiddlewareRegisterExtension
{
    public static void CurrentUser(this IApplicationBuilder app)
    {
        app.UseMiddleware<UserMiddleware>();
    }
}
