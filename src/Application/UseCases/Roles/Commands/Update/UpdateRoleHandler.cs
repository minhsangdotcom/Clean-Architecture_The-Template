using Application.Common.Exceptions;
using Application.Common.Interfaces.Services.Identity;
using Application.UseCases.Projections.Roles;
using AutoMapper;
using Contracts.Common.Messages;
using Domain.Aggregates.Roles;
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
            await roleManagerService.GetByIdAsync(Ulid.Parse(command.RoleId))
            ?? throw new NotFoundException(
                [Messager.Create<Role>().Message(MessageType.Found).Negative().BuildMessage()]
            );

        mapper.Map(command.Role, role);

        List<RoleClaimModel>? claims = command.Role.RoleClaims;
        if (claims?.Count > 0)
        {
            List<RoleClaim> roleClaims = mapper.Map<List<RoleClaim>>(claims);
            await roleManagerService.UpdateRoleAsync(role, roleClaims);
        }

        return mapper.Map<UpdateRoleResponse>(role);
    }
}
