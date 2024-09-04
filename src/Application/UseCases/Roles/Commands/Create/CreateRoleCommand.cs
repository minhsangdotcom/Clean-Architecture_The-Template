using Application.UseCases.Projections.Roles;
using Mediator;

namespace Application.UseCases.Roles.Commands.Create;

public class CreateRoleCommand : RoleModel,IRequest<CreateRoleResponse>
{
    public List<RoleClaimModel>? RoleClaims { get; set; }
}