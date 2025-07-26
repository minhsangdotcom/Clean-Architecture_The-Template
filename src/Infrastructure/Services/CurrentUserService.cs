using System.Security.Claims;
using Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class CurrentUserService : ICurrentUser
{
    public Ulid? Id { get; private set; }

    public string? ClientIp { get; private set; }

    public void Set(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated == false)
        {
            Id = null;
            return;
        }
        
        if (Ulid.TryParse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Ulid id))
        {
            Id = id;
        }
    }

    public void SetClientIp(HttpContext httpContext)
    {
        ClientIp = httpContext.Connection.RemoteIpAddress?.ToString();
    }
}