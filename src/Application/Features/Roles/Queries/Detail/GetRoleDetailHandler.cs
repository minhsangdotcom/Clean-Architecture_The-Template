using Application.Common.Exceptions;
using Application.Common.Interfaces.Services.Identity;
using AutoMapper;
using Contracts.Common.Messages;
using Domain.Aggregates.Roles;
using Mediator;

namespace Application.Features.Roles.Queries.Detail;

public class GetRoleDetailHandler(IRoleManagerService roleManagerService, IMapper mapper)
    : IRequestHandler<GetRoleDetailQuery, RoleDetailResponse>
{
    public async ValueTask<RoleDetailResponse> Handle(
        GetRoleDetailQuery query,
        CancellationToken cancellationToken
    ) =>
        mapper.Map<RoleDetailResponse>(
            await roleManagerService.FindByIdAsync(query.Id)
                ?? throw new NotFoundException(
                    [Messager.Create<Role>().Message(MessageType.Found).Negative().Build()]
                )
        );
}
