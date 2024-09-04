using Application.UseCases.Roles.Commands.Update;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Routers;
using Contracts.Routes;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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