using Application.Common.Interfaces.Services;

namespace Api.Middlewares;

public class UserMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        var currentUser = context.RequestServices.GetRequiredService<ICurrentUser>();
        currentUser.Set(context.User);
        currentUser.SetClientIp(context);
        
        await next.Invoke(context);
    }
}