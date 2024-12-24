using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Contracts.Constants;
using Contracts.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

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

        AuthorizeModel? authorizeModel = SerializerExtension
            .Deserialize<AuthorizeModel>(requirement.Requirement())
            .Object;

        if (
            authorizeModel == null
            || authorizeModel!.Permissions?.Count == 0
            || authorizeModel!.Roles?.Count == 0
        )
        {
            context.Succeed(requirement);
            return;
        }

        if (authorizeModel.Roles!.Count > 0 && authorizeModel.Permissions!.Count > 0)
        {
            SuccessOrFailiureHandler(
                context,
                requirement,
                await userManagerService.HasClaimsAndRoleInUserAsync(
                    userId.Value,
                    authorizeModel.Roles,
                    authorizeModel.Permissions.Select(permission => new KeyValuePair<
                        string,
                        string
                    >(ClaimTypes.Permission, permission))
                )
            );
            return;
        }

        if (authorizeModel.Roles!.Count > 0)
        {
            SuccessOrFailiureHandler(
                context,
                requirement,
                await userManagerService.HasRolesInUserAsync(userId.Value, authorizeModel.Roles)
            );

            return;
        }

        if (authorizeModel.Permissions!.Count > 0)
        {
            SuccessOrFailiureHandler(
                context,
                requirement,
                await userManagerService.HasClaimsInUserAsync(
                    userId.Value,
                    authorizeModel.Permissions.Select(permission => new KeyValuePair<
                        string,
                        string
                    >(ClaimTypes.Permission, permission))
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
}
