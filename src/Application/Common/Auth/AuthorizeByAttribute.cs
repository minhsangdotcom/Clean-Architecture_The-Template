using Application.Common.Security;
using Contracts.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Auth;

public class AuthorizeByAttribute : AuthorizeAttribute
{
    public string Value
    {
        get { return Policy![AuthorizePolicy.POLICY_PREFIX.Length..] ?? string.Empty; }
        set { Policy = $"{AuthorizePolicy.POLICY_PREFIX}{value}"; }
    }

    public AuthorizeByAttribute(string? roles = null, string? permissions = null)
    {
        Value = string.Empty;
        if (!string.IsNullOrWhiteSpace(roles) || !string.IsNullOrWhiteSpace(permissions))
        {
            AuthorizeModel authorizeModel =
                new()
                {
                    Roles = roles?.Trim()?.Split(',', StringSplitOptions.TrimEntries)?.ToList(),
                    Permissions = permissions
                        ?.Trim()
                        ?.Split(',', StringSplitOptions.TrimEntries)
                        ?.ToList(),
                };
            Value = SerializerExtension.Serialize(authorizeModel).StringJson;
        }
    }
}
