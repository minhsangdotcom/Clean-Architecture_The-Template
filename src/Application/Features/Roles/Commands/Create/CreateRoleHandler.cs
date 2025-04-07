using Application.Common.Interfaces.Services.Identity;
using Domain.Aggregates.Roles;
using Mediator;

namespace Application.Features.Roles.Commands.Create;

public class CreateRoleHandler(IRoleManagerService roleManagerService)
    : IRequestHandler<CreateRoleCommand, CreateRoleResponse>
{
    public async ValueTask<CreateRoleResponse> Handle(
        CreateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        Role mappingRole = command.ToRole();
        Role role = await roleManagerService.CreateAsync(mappingRole);
        return role.ToCreateRoleResponse();
    }
}
