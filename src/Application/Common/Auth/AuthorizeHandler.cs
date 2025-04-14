using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Constants;
using SharedKernel.Extensions;

namespace Application.Common.Auth;

public class AuthorizeHandler(IServiceProvider serviceProvider)
    : AuthorizationHandler<AuthorizationRequirement>
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

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
            context.Fail(new AuthorizationFailureReason(this, "User is UnAuthenticated"));
            return;
        }

        string? authorize = requirement.Requirement();
        AuthorizeModel? authorizeModel = null;
        if (!string.IsNullOrWhiteSpace(authorize))
        {
            authorizeModel = SerializerExtension.Deserialize<AuthorizeModel>(authorize).Object;
        }

        if (
            authorizeModel == null
            || (authorizeModel!.Permissions?.Count == 0 && authorizeModel!.Roles?.Count == 0)
        )
        {
            context.Succeed(requirement);
            return;
        }

        if (authorizeModel.Roles?.Count > 0 && authorizeModel.Permissions?.Count > 0)
        {
            bool hasRolesAndClaims = await userManagerService.HasUserClaimsAndRolesAsync(
                userId.Value,
                authorizeModel.Roles,
                authorizeModel.Permissions.Select(permission => new KeyValuePair<string, string>(
                    ClaimTypes.Permission,
                    permission
                ))
            );
            SuccessOrFailiureHandler(context, requirement, hasRolesAndClaims);
            return;
        }

        if (authorizeModel.Roles?.Count > 0)
        {
            bool hasRole = await userManagerService.HasUserRolesAsync(
                userId.Value,
                authorizeModel.Roles
            );
            SuccessOrFailiureHandler(context, requirement, hasRole);

            return;
        }

        if (authorizeModel.Permissions?.Count > 0)
        {
            bool hasPermission = await userManagerService.HasUserPermissionAsync(
                userId.Value,
                authorizeModel.Permissions
            );
            SuccessOrFailiureHandler(context, requirement, hasPermission);

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
}
