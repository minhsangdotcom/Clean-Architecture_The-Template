using Application.Features.Common.Projections.Roles;
using Contracts.Routers;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Application.Features.Roles.Commands.Update;

public class UpdateRoleCommand : IRequest<UpdateRoleResponse>
{
    [FromRoute(Name = Router.Id)]
    public string RoleId { get; set; } = string.Empty;

    [FromBody]
    public UpdateRole Role { get; set; } = null!;
}

public class UpdateRole : RoleModel;
