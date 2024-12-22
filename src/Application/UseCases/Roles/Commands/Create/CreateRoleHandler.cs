using Application.Common.Interfaces.Services.Identity;
using AutoMapper;
using Domain.Aggregates.Roles;
using Mediator;

namespace Application.UseCases.Roles.Commands.Create;

public class CreateRoleHandler(IRoleManagerService roleManagerService, IMapper mapper)
    : IRequestHandler<CreateRoleCommand, CreateRoleResponse>
{
    public async ValueTask<CreateRoleResponse> Handle(
        CreateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        Role mappingRole = mapper.Map<Role>(command);
        Role role = await roleManagerService.CreateRoleAsync(mappingRole);
        return mapper.Map<CreateRoleResponse>(role);
    }
}
