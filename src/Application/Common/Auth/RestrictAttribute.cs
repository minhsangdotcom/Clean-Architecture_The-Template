using Application.Common.Security;
using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Auth;

public class RestrictAttribute : AuthorizeAttribute
{
    public string Value
    {
        get { return Policy![AuthorizePolicy.POLICY_PREFIX.Length..] ?? string.Empty; }
        set { Policy = $"{AuthorizePolicy.POLICY_PREFIX}{value}"; }
    }

    public RestrictAttribute(string? roles = null, string? claims = null)
    {
        const string dash = "-";

        Value = true switch
        {
            bool when !string.IsNullOrWhiteSpace(roles) && !string.IsNullOrWhiteSpace(claims) =>
                roles + dash + claims,
            bool when !string.IsNullOrWhiteSpace(roles) => roles + dash,
            bool when !string.IsNullOrWhiteSpace(claims) => dash + claims,
            _ => string.Empty,
        };
    }
}
