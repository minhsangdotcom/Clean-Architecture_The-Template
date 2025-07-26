using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Services;

public interface ICurrentUser
{
    public Ulid? Id { get; }

    public string? ClientIp { get; }

    void SetClientIp(HttpContext httpContext);

    void Set(ClaimsPrincipal user);
}
