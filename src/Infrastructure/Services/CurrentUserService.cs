using System.Security.Claims;
using Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Ulid? Id => GetId(ClaimTypes.NameIdentifier);

    public string? ClientIp =>
        httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    public string? AuthenticationScheme =>
        httpContextAccessor
            .HttpContext?.Features.Get<IAuthenticateResultFeature>()
            ?.AuthenticateResult?.Ticket?.AuthenticationScheme
        ?? JwtBearerDefaults.AuthenticationScheme;

    private Ulid? GetId(string claimType)
    {
        string? id = httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType);

        if (id is null)
        {
            return Ulid.Empty;
        }

        return Ulid.Parse(id);
    }
}
