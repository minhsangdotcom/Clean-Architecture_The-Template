using Application.Common.Exceptions;
using Application.Common.Interfaces.Services.Identity;
using Application.Features.Common.Mapping.Roles;
using Domain.Aggregates.Roles;
using Mediator;
using SharedKernel.Common.Messages;

namespace Application.Features.Roles.Commands.Update;

public class UpdateRoleHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<UpdateRoleCommand, UpdateRoleResponse>
{
    public async ValueTask<UpdateRoleResponse> Handle(
        UpdateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        Role role =
            await roleManagerService.GetByIdAsync(Ulid.Parse(command.RoleId))
            ?? throw new NotFoundException(
                [Messager.Create<Role>().Message(MessageType.Found).Negative().BuildMessage()]
            );

        role.FromUpdateRole(command.Role);

        List<RoleClaim> roleClaims = command.Role.RoleClaims.ToListRoleClaim() ?? [];
        await roleManagerService.UpdateRoleAsync(role, roleClaims);
        return role.ToUpdateRoleResponse();
    }
}
