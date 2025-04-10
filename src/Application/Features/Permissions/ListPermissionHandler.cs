using Application.Common.Interfaces.Services.Identity;
using Contracts.ApiWrapper;
using Domain.Aggregates.Roles;
using Mediator;

namespace Application.Features.Permissions;

public class ListPermissionHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<ListPermissionQuery, Result<IEnumerable<ListPermissionResponse>>>
{
    public async ValueTask<Result<IEnumerable<ListPermissionResponse>>> Handle(
        ListPermissionQuery request,
        CancellationToken cancellationToken
    )
    {
        IList<RoleClaim> roleClaims = await roleManagerService.GetRolePermissionClaimsAsync();
        IEnumerable<ListPermissionResponse> responses = roleClaims.Select(
            claim => new ListPermissionResponse()
            {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue,
            }
        );
        return Result<IEnumerable<ListPermissionResponse>>.Success(responses);
    }
}
