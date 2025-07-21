using Application.Common.Interfaces.Services.Identity;
using Contracts.ApiWrapper;
using Mediator;
using SharedKernel.Constants;

namespace Application.Features.Permissions;

public class ListPermissionHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<ListPermissionQuery, Result<IEnumerable<ListPermissionResponse>>>
{
    public async ValueTask<Result<IEnumerable<ListPermissionResponse>>> Handle(
        ListPermissionQuery request,
        CancellationToken cancellationToken
    )
    {
        var roleClaims = await roleManagerService.GetRolePermissionClaimsAsync();
        var responses = roleClaims.SelectMany(claim =>
            claim.Select(parent => new ListPermissionResponse()
            {
                ClaimType = ClaimTypes.Permission,
                ClaimValue = parent.Key,
                Children = parent.Value.ConvertAll(child => new PermissionResponse()
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = child,
                }),
            })
        );
        return Result<IEnumerable<ListPermissionResponse>>.Success(responses);
    }
}
