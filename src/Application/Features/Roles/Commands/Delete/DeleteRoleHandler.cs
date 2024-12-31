using Application.Common.Exceptions;
using Application.Common.Interfaces.Services.Identity;
using Contracts.Common.Messages;
using Domain.Aggregates.Roles;
using Mediator;

namespace Application.Features.Roles.Commands.Delete;

public class DeleteRoleHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<DeleteRoleCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        Role role =
            await roleManagerService.FindByIdAsync(command.RoleId)
            ?? throw new NotFoundException(
                [Messager.Create<Role>().Message(MessageType.Found).Negative().BuildMessage()]
            );

        await roleManagerService.DeleteRoleAsync(role);

        return Unit.Value;
    }
}
