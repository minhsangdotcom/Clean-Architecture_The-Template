using Application.Common.Interfaces.Services.Identity;
using Mediator;

namespace Application.Features.Roles.Queries.List;

public class ListRoleHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<ListRoleQuery, IEnumerable<ListRoleResponse>>
{
    public async ValueTask<IEnumerable<ListRoleResponse>> Handle(
        ListRoleQuery query,
        CancellationToken cancellationToken
    ) => (await roleManagerService.ListAsync()).ToListRoleResponse();
}
