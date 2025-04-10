using Application.Common.Errors;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Services.Identity;
using Contracts.ApiWrapper;
using Domain.Aggregates.Roles;
using Mediator;
using SharedKernel.Common.Messages;

namespace Application.Features.Roles.Queries.Detail;

public class GetRoleDetailHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<GetRoleDetailQuery, Result<RoleDetailResponse>>
{
    public async ValueTask<Result<RoleDetailResponse>> Handle(
        GetRoleDetailQuery query,
        CancellationToken cancellationToken
    )
    {
        Role? role = await roleManagerService.FindByIdAsync(query.Id);

        if (role == null)
        {
            return Result<RoleDetailResponse>.Failure(
                new NotFoundError(
                    "Your resource is not found",
                    Messager.Create<Role>().Message(MessageType.Found).Negative().Build()
                )
            );
        }
        return Result<RoleDetailResponse>.Success(role.ToRoleDetailResponse());
    }
}
