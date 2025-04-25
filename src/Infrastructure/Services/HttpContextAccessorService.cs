using Application.Common.Interfaces.Services;
using Contracts.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Infrastructure.Services;

public class HttpContextAccessorService(IHttpContextAccessor httpContextAccessor)
    : IHttpContextAccessorService
{
    public HttpContext? HttpContext => httpContextAccessor.HttpContext;

    public string? GetRouteValue(string key)
    {
        return HttpContext?.GetRouteValue(key)?.ToString();
    }

    public string? GetHttpMethod()
    {
        return HttpContext?.Request.Method;
    }

    public string? GetRequestPath()
    {
        return HttpContext?.Request.Path;
    }

    public string? GetId()
    {
        return GetRouteValue(RoutePath.Id);
    }
}
