using Api.common.RouteResults;
using Api.common.Routers;
using Application.Common.Auth;
using Application.Features.Roles.Queries.List;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Infrastructure.Constants;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.Roles;

public class ListRoleEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<ListRoleQuery>.WithActionResult<
        ApiResponse<IEnumerable<ListRoleResponse>>
    >
{
    [HttpGet(Router.RoleRoute.Roles)]
    [SwaggerOperation(Tags = [Router.RoleRoute.Tags], Summary = "List Role")]
    [AuthorizeBy(permissions: $"{ActionPermission.list}:{ObjectPermission.role}")]
    public override async Task<
        ActionResult<ApiResponse<IEnumerable<ListRoleResponse>>>
    > HandleAsync(
        [FromQuery] ListRoleQuery request,
        CancellationToken cancellationToken = default
    ) => this.Ok200(await sender.Send(request, cancellationToken));
}
