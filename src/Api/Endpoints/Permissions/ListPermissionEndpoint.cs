using Application.Features.Permissions;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.RouteResults;
using Contracts.Routers;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.Permissions;

public class ListPermissionEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<ListPermissionQuery>.WithActionResult<ApiResponse>
{
    [HttpGet(Router.PermissionRoute.Permissions)]
    [SwaggerOperation(Tags = [Router.PermissionRoute.Tags], Summary = "List permissions")]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(
        [FromQuery] ListPermissionQuery request,
        CancellationToken cancellationToken = default
    )
    {
        return this.Ok200(await sender.Send(request, cancellationToken));
    }
}
