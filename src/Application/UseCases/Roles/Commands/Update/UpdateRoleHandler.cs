using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Aggregates.Users;
using Mediator;

namespace Application.UseCases.Roles.Commands.Update;

public class UpdateRoleHandler(IRoleManagerService roleManagerService, IMapper mapper) : IRequestHandler<UpdateRoleCommand, UpdateRoleResponse>
{
    public async ValueTask<UpdateRoleResponse> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        Role role = await roleManagerService.FindByIdAsync(Ulid.Parse(command.RoleId));
        mapper.Map(command.Role, role);

        List<RoleClaim> roleClaims = mapper.Map<List<RoleClaim>>(command.Role.RoleClaims) ?? [];
        roleClaims.ForEach(x => x.RoleId = role.Id);

        await roleManagerService.UpdateRoleAsync(role, roleClaims);
        return mapper.Map<UpdateRoleResponse>(role);
    }
}