using Application.Common.Exceptions;
using Application.Common.Interfaces.Services.Identity;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using Mediator;

namespace Application.UseCases.Roles.Commands.Delete;

public class DeleteRoleHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<DeleteRoleCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        Role role =
            await roleManagerService.FindByIdAsync(command.UserId)
            ?? throw new BadRequestException(
                [Messager.Create<User>().Message(MessageType.Found).Negative().BuildMessage()]
            );

        await roleManagerService.DeleteRoleAsync(role);

        return Unit.Value;
    }
}
