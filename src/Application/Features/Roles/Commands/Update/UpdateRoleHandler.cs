using Application.Common.Exceptions;
using Application.Common.Interfaces.Services.Identity;
using AutoMapper;
using Domain.Aggregates.Roles;
using Mediator;
using SharedKernel.Common.Messages;

namespace Application.Features.Roles.Commands.Update;

public class UpdateRoleHandler(IRoleManagerService roleManagerService, IMapper mapper)
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

        mapper.Map(command.Role, role);

        List<RoleClaim> roleClaims = mapper.Map<List<RoleClaim>>(command.Role.RoleClaims);
        await roleManagerService.UpdateRoleAsync(role, roleClaims);
        return mapper.Map<UpdateRoleResponse>(role);
    }
}
