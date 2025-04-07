using Application.Common.Interfaces.Services.Identity;
using Domain.Aggregates.Roles;
using Mediator;

namespace Application.Features.Permissions;

public class ListPermissionHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<ListPermissionQuery, IEnumerable<ListPermissionResponse>>
{
    public async ValueTask<IEnumerable<ListPermissionResponse>> Handle(
        ListPermissionQuery request,
        CancellationToken cancellationToken
    )
    {
        IList<RoleClaim> roleClaims = await roleManagerService.GetRolePermissionClaimsAsync();
        return roleClaims.Select(x => new ListPermissionResponse()
        {
            ClaimType = x.ClaimType,
            ClaimValue = x.ClaimValue,
        });
    }
}
