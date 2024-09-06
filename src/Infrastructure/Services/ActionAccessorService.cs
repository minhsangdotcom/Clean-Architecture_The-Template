using Application.Common.Interfaces.Services;
using Contracts.Routers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Infrastructure.Services;

public class ActionAccessorService(IActionContextAccessor actionContextAccessor) : IActionAccessorService
{
    public string? Id => GetRouteData(Router.Id);

    public string? GetAction() =>
        actionContextAccessor.ActionContext?.ActionDescriptor?.RouteValues["action"];

    public string? GetController() =>
        actionContextAccessor.ActionContext?.ActionDescriptor?.RouteValues["controller"];

    public string? GetHttpMethod() =>
        actionContextAccessor?.ActionContext?.HttpContext.Request.Method;

    public string? GetRouteData(string name) =>
        actionContextAccessor.ActionContext?.RouteData.Values[name]?.ToString();

    public string? GetRequestPath()
        => actionContextAccessor.ActionContext?.HttpContext.Request.Path;
}