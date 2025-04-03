using Application.Common.Exceptions;
using Application.Common.Interfaces.Services.Identity;
using Domain.Aggregates.Roles;
using Mediator;
using SharedKernel.Common.Messages;

namespace Application.Features.Roles.Queries.Detail;

public class GetRoleDetailHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<GetRoleDetailQuery, RoleDetailResponse>
{
    public async ValueTask<RoleDetailResponse> Handle(
        GetRoleDetailQuery query,
        CancellationToken cancellationToken
    )
    {
        Role role =
            await roleManagerService.FindByIdAsync(query.Id)
            ?? throw new NotFoundException(
                [Messager.Create<Role>().Message(MessageType.Found).Negative().Build()]
            );
        return role.ToRoleDetailResponse();
    }
}
