using System.Security.Claims;
using Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class CurrentUserService : ICurrentUser
{
    public Ulid? Id { get; private set; }

    public string? ClientIp { get; private set; }

    private ClaimsPrincipal user = null!;

    public void SetClaimPrinciple(ClaimsPrincipal user)
    {
        this.user ??= user;

        string? id = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrWhiteSpace(id))
        {
            Id = Ulid.Parse(id);
        }
    }

    public void SetClientIp(HttpContext httpContext)
    {
        ClientIp = httpContext.Connection.RemoteIpAddress?.ToString();
    }
}
