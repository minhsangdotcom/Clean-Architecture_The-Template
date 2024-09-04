using Application.Common.Interfaces.Services;
using AutoMapper;
using Mediator;

namespace Application.UseCases.Roles.Queries.List;

public class ListRoleHandler(IRoleManagerService roleManagerService, IMapper mapper) : IRequestHandler<ListRoleQuery, IEnumerable<ListRoleResponse>>
{
    public async ValueTask<IEnumerable<ListRoleResponse>> Handle(ListRoleQuery request, CancellationToken cancellationToken) =>
        mapper.Map<IEnumerable<ListRoleResponse>>(await roleManagerService.ListAsync());
}