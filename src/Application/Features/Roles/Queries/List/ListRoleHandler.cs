using Application.Common.Interfaces.Services.Identity;
using AutoMapper;
using Mediator;

namespace Application.Features.Roles.Queries.List;

public class ListRoleHandler(IRoleManagerService roleManagerService, IMapper mapper)
    : IRequestHandler<ListRoleQuery, IEnumerable<ListRoleResponse>>
{
    public async ValueTask<IEnumerable<ListRoleResponse>> Handle(
        ListRoleQuery query,
        CancellationToken cancellationToken
    ) => mapper.Map<IEnumerable<ListRoleResponse>>(await roleManagerService.ListAsync());
}
