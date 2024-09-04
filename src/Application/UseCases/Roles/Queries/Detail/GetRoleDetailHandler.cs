using Application.Common.Interfaces.Services;
using AutoMapper;
using Mediator;

namespace Application.UseCases.Roles.Queries.Detail;

public class GetRoleDetailHandler(IRoleManagerService roleManagerService, IMapper mapper) : IRequestHandler<GetRoleDetailQuery, RoleDetailResponse>
{
    public async ValueTask<RoleDetailResponse> Handle(GetRoleDetailQuery query, CancellationToken cancellationToken) =>
        mapper.Map<RoleDetailResponse>(await roleManagerService.FindByIdAsync(query.Id));
}