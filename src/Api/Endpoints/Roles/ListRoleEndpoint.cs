using Application.Features.Roles.Queries.List;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Common.Messages;
using Contracts.Routers;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.Roles;

public class ListRoleEndpoint(ISender sender) : EndpointBaseAsync.WithRequest<ListRoleQuery>.WithActionResult<ApiResponse>
{
    [HttpGet(Router.RoleRoute.Roles)]
    [SwaggerOperation(
            Tags = [Router.RoleRoute.Tags],
            Summary = "List Role"
        )]
    public override async Task<ActionResult<ApiResponse>> HandleAsync([FromQuery]ListRoleQuery request, CancellationToken cancellationToken = default) =>
        new ApiResponse(await sender.Send(request, cancellationToken), Message.SUCCESS);
}