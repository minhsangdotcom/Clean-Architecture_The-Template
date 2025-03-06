using Api.common.RouteResults;
using Api.common.Routers;
using Application.Features.Regions.Queries.List.Districts;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.Regions;

public class ListDistrictEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<ListDistrictQuery>.WithActionResult<ApiResponse>
{
    [HttpGet(Router.RegionRoute.Districts)]
    [SwaggerOperation(Tags = [Router.RegionRoute.Tags], Summary = "list District")]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(
        ListDistrictQuery request,
        CancellationToken cancellationToken = default
    ) => this.Ok200(await sender.Send(request, cancellationToken));
}
