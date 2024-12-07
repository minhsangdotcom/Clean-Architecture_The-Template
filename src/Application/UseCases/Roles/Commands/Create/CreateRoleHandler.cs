using Application.Common.Interfaces.Services.Identity;
using AutoMapper;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Users;
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
        Role mappingrole = mapper.Map<Role>(command);
        Role role = await roleManagerService.CreateRoleAsync(mappingrole);
        return mapper.Map<CreateRoleResponse>(role);
    }
}
