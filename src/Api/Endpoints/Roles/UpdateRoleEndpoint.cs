using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Routers;
using Contracts.RouteResults;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Application.Features.Roles.Commands.Update;

namespace Api.Endpoints.Roles;

public class UpdateRoleEndpoint(ISender sender) : EndpointBaseAsync.WithRequest<UpdateRoleCommand>.WithActionResult<ApiResponse>
{
    [HttpPut(Router.RoleRoute.GetUpdateDelete)]
    [SwaggerOperation(
            Tags = [Router.RoleRoute.Tags],
            Summary = "update Role"
        )]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(UpdateRoleCommand request, CancellationToken cancellationToken = default) =>
        this.Ok200(await sender.Send(request, cancellationToken));
}