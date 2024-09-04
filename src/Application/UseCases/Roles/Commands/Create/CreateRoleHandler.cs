using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Aggregates.Users;
using Mediator;

namespace Application.UseCases.Roles.Commands.Create;

public class CreateRoleHandler(
    IRoleManagerService roleManagerService,
    IMapper mapper) : IRequestHandler<CreateRoleCommand, CreateRoleResponse>
{
    public async ValueTask<CreateRoleResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken) =>
        mapper.Map<CreateRoleResponse>(await roleManagerService.CreateRoleAsync(mapper.Map<Role>(request)));
}