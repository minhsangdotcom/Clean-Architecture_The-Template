using Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Auth;

public class AuthorizeHandler(IServiceProvider serviceProvider)
    : AuthorizationHandler<AuthorizationRequirement>
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    private static RequireTypeResponse GetRequireType(string requirement)
    {
        if (string.IsNullOrWhiteSpace(requirement))
        {
            return new(RequireType.None);
        }

        string[] require = requirement.Trim().Split("-", StringSplitOptions.RemoveEmptyEntries);

        if (requirement.StartsWith('-'))
        {
            return new(RequireType.Claim, require);
        }

        if (requirement.EndsWith('-'))
        {
            return new(RequireType.Role, require);
        }

        return new(RequireType.Both, require);
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthorizationRequirement requirement
    )
    {
        using var scope = serviceProvider.CreateScope();
        IUserManagerService userManagerService =
            scope.ServiceProvider.GetRequiredService<IUserManagerService>();
        ICurrentUser currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>();

        Ulid? userId = currentUser.Id;

        if (userId == null)
        {
            context.Fail(new AuthorizationFailureReason(this, "User UnAuthenticated"));
            return;
        }

        (RequireType type, string[]? require) = GetRequireType(requirement.Requirement());

        if (type == RequireType.None && require == null)
        {
            context.Succeed(requirement);
            return;
        }

        IEnumerable<string> requireData = require![0].Trim().Split(",").Select(x => x.Trim());

        if (type == RequireType.Both && require.Length == 2)
        {
            IEnumerable<string> claims = require[1].Trim().Split(",").Select(x => x.Trim());
            SuccessOrFailiureHandler(
                context,
                requirement,
                claims.Any(x => x.Contains(':'))
                    && await userManagerService.HasClaimsAndRoleInUserAsync(
                        userId.Value,
                        requireData,
                        GetClaimKeyValues(claims)
                    )
            );
        }

        if (type == RequireType.Role)
        {
            SuccessOrFailiureHandler(
                context,
                requirement,
                await userManagerService.HasRolesInUserAsync(userId.Value, requireData)
            );

            return;
        }

        if (type == RequireType.Claim)
        {
            SuccessOrFailiureHandler(
                context,
                requirement,
                requireData.Any(x => x.Contains(':'))
                    && await userManagerService.HasClaimsInUserAsync(
                        userId.Value,
                        GetClaimKeyValues(requireData)
                    )
            );

            return;
        }

        await Task.CompletedTask;
    }

    private static void SuccessOrFailiureHandler(
        AuthorizationHandlerContext context,
        AuthorizationRequirement requirement,
        bool isSuccess = false
    )
    {
        if (!isSuccess)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }

    private static Dictionary<string, string> GetClaimKeyValues(IEnumerable<string> claims)
    {
        return claims
            .Select(claim =>
            {
                var claimArr = claim.Split(":");
                return new { Key = claimArr.First(), Value = claimArr.Last() };
            })
            .ToDictionary(x => x.Key, x => x.Value);
    }

    private enum RequireType
    {
        None = 0,
        Role = 1,
        Claim = 2,
        Both = 3,
    }

    private record RequireTypeResponse(RequireType Type, string[]? Requirement = null);
}
