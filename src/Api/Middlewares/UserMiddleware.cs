using Application.Common.Interfaces.Services;

namespace Api.Middlewares.GlobalExceptionHandlers;

public class UserMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, ICurrentUser currentUser)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            currentUser.SetClaimPrinciple(context.User);
        }
        
        currentUser.SetClientIp(context);

        await next.Invoke(context);
    }
}