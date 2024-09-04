using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Domain.Aggregates.Users;
using Mediator;

namespace Application.UseCases.Roles.Commands.Delete;

public class DeleteRoleHandler(IRoleManagerService roleManagerService) : IRequestHandler<DeleteRoleCommand>
{
    public async ValueTask<Unit> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        Role role = await roleManagerService.FindByIdAsync(command.UserId) ??
            throw new BadRequestException($"{nameof(Role).ToUpper()}_NOTFOUND");

        await roleManagerService.DeleteRoleAsync(role);

        return Unit.Value;
    }
}