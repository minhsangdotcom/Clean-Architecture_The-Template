using Api.common.RouteResults;
using Api.common.Routers;
using Application.Common.Auth;
using Application.Features.Roles.Commands.Update;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Infrastructure.Constants;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.Roles;

public class UpdateRoleEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<UpdateRoleCommand>.WithActionResult<ApiResponse>
{
    [HttpPut(Router.RoleRoute.GetUpdateDelete)]
    [SwaggerOperation(Tags = [Router.RoleRoute.Tags], Summary = "update Role")]
    [AuthorizeBy(permissions: $"{ActionPermission.update}:{ObjectPermission.role}")]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(
        UpdateRoleCommand request,
        CancellationToken cancellationToken = default
    ) => this.Ok200(await sender.Send(request, cancellationToken));
}
