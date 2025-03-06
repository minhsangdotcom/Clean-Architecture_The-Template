using Application.Common.Interfaces.Services.Identity;
using Mediator;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Constants;

namespace Application.Features.Permissions;

public class ListPermissionHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<ListPermissionQuery, IEnumerable<ListPermissionResponse>>
{
    public async ValueTask<IEnumerable<ListPermissionResponse>> Handle(
        ListPermissionQuery request,
        CancellationToken cancellationToken
    )
    {
        return (
            await roleManagerService
                .RoleClaims.Where(x => x.ClaimType == ClaimTypes.Permission)
                .ToListAsync(cancellationToken)
        ).Select(x => new ListPermissionResponse()
        {
            ClaimType = x.ClaimType,
            ClaimValue = x.ClaimValue,
        });
    }
}
