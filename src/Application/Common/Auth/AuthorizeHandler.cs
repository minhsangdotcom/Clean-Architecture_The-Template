using Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Auth;

public class AuthorizeHandler(IServiceProvider serviceProvider) : AuthorizationHandler<AuthorizationRequirement>
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

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
    {
        using var scope = serviceProvider.CreateScope();
        IUserManagerService userManagerService = scope.ServiceProvider.GetRequiredService<IUserManagerService>();
        ICurrentUser currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>();

        Ulid? userId = currentUser.Id;

        if (userId == null)
        {
            context.Fail(new AuthorizationFailureReason(this, "User UnAuthenticated"));
            return;
        }

        (RequireType type, string[]? require) = GetRequireType(requirement.Requirement());

        if (type == RequireType.None)
        {
            context.Succeed(requirement);
            return;
        }

        string[] requireData = require![0].Trim().Split(",");
        
        if (require.Length == 2)
        {
            SuccessOrFailiureHandler(
                context,
                requirement,
                await userManagerService.HasClaimsAndRoleInUserAsync(
                    userId.Value,
                    requireData,
                    require[1].Trim().Split(",")
                )
            );
        }

        if (type == RequireType.Role)
        {
            SuccessOrFailiureHandler(
                context,
                requirement,
                await userManagerService.HasRolesInUserAsync(userId.Value, requireData));

            return;
        }

        if (type == RequireType.Claim)
        {
            SuccessOrFailiureHandler(
                context,
                requirement,
                await userManagerService.HasClaimsInUserAsync(userId.Value, requireData)
            );

            return;
        }

        await Task.CompletedTask;
    }

    private static void SuccessOrFailiureHandler(AuthorizationHandlerContext context, AuthorizationRequirement requirement, bool isSuccess = false)
    {
        if (!isSuccess)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
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