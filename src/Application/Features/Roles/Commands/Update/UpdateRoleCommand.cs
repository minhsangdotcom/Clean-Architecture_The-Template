using Application.Features.Common.Projections.Roles;
using Contracts.ApiWrapper;
using Contracts.Constants;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Application.Features.Roles.Commands.Update;

public class UpdateRoleCommand : IRequest<Result<UpdateRoleResponse>>
{
    [FromRoute(Name = RoutePath.Id)]
    public string RoleId { get; set; } = string.Empty;

    [FromBody]
    public UpdateRole Role { get; set; } = null!;
}

public class UpdateRole : RoleModel;
