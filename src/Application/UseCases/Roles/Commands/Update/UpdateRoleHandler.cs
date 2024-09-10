using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using AutoMapper;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using Mediator;

namespace Application.UseCases.Roles.Commands.Update;

public class UpdateRoleHandler(IRoleManagerService roleManagerService, IMapper mapper)
    : IRequestHandler<UpdateRoleCommand, UpdateRoleResponse>
{
    public async ValueTask<UpdateRoleResponse> Handle(
        UpdateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        Role role =
            await roleManagerService.FindByIdAsync(Ulid.Parse(command.RoleId))
            ?? throw new BadRequestException(
                [Messager.Create<Role>().Message(MessageType.Found).Negative().BuildMessage()]
            );

        mapper.Map(command.Role, role);

        List<RoleClaim> roleClaims = mapper.Map<List<RoleClaim>>(command.Role.Claims) ?? [];
        roleClaims.ForEach(x => x.RoleId = role.Id);

        await roleManagerService.UpdateRoleAsync(role, roleClaims);
        return mapper.Map<UpdateRoleResponse>(role);
    }
}
