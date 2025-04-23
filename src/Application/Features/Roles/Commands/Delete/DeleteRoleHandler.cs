using Application.Common.Errors;
using Application.Common.Interfaces.Services.Identity;
using Contracts.ApiWrapper;
using Domain.Aggregates.Roles;
using Mediator;
using SharedKernel.Common.Messages;

namespace Application.Features.Roles.Commands.Delete;

public class DeleteRoleHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<DeleteRoleCommand, Result<string>>
{
    public async ValueTask<Result<string>> Handle(
        DeleteRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        Role? role = await roleManagerService.FindByIdAsync(command.RoleId);

        if (role == null)
        {
            return Result<string>.Failure(
                new NotFoundError(
                    "Your Resource is not found",
                    Messager.Create<Role>().Message(MessageType.Found).Negative().BuildMessage()
                )
            );
        }

        await roleManagerService.DeleteAsync(role);

        return Result<string>.Success();
    }
}
