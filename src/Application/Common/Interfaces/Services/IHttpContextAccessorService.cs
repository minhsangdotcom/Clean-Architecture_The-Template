using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Services;

public interface IHttpContextAccessorService
{
    HttpContext? HttpContext { get; }
    string? GetRouteValue(string key);
    string? GetHttpMethod();
    string? GetRequestPath();
    string? GetId();
}
