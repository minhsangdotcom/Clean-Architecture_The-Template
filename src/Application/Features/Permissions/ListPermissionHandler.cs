using Application.Common.Interfaces.Services.Identity;
using Contracts.ApiWrapper;
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
        IList<KeyValuePair<string, string>> roleClaims =
            await roleManagerService.GetRolePermissionClaimsAsync();
        IEnumerable<ListPermissionResponse> responses = roleClaims.Select(
            claim => new ListPermissionResponse()
            {
                ClaimType = claim.Key,
                ClaimValue = claim.Value,
            }
        );
        return Result<IEnumerable<ListPermissionResponse>>.Success(responses);
    }
}
