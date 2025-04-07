using Application.Common.Interfaces.Services.Identity;
using Domain.Aggregates.Roles;
using Mediator;

namespace Application.Features.Roles.Queries.List;

public class ListRoleHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<ListRoleQuery, IEnumerable<ListRoleResponse>>
{
    public async ValueTask<IEnumerable<ListRoleResponse>> Handle(
        ListRoleQuery query,
        CancellationToken cancellationToken
    )
    {
        List<Role> roles = await roleManagerService.ListAsync();
        return roles.ToListRoleResponse();
    }
}
