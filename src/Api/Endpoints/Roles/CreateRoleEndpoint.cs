using Api.common.RouteResults;
using Api.common.Routers;
using Application.Common.Auth;
using Application.Features.Roles.Commands.Create;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Infrastructure.Constants;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.Roles;

public class CreateRoleEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<CreateRoleCommand>.WithActionResult<
        ApiResponse<CreateRoleResponse>
    >
{
    [HttpPost(Router.RoleRoute.Roles)]
    [SwaggerOperation(Tags = [Router.RoleRoute.Tags], Summary = "create Role")]
    [AuthorizeBy(permissions: $"{ActionPermission.create}:{ObjectPermission.role}")]
    public override async Task<ActionResult<ApiResponse<CreateRoleResponse>>> HandleAsync(
        CreateRoleCommand request,
        CancellationToken cancellationToken = default
    )
    {
        CreateRoleResponse role = await sender.Send(request, cancellationToken);
        return this.Created201(Router.RoleRoute.GetRouteName, role.Id, role);
    }
}
