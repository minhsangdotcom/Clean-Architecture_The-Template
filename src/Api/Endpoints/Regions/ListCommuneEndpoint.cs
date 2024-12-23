using Application.Features.Regions.Queries.List.Communes;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.RouteResults;
using Contracts.Routers;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.Regions;

public class ListCommuneEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<ListCommuneQuery>.WithActionResult<ApiResponse>
{
    [HttpGet(Router.RegionRoute.Communes)]
    [SwaggerOperation(Tags = [Router.RegionRoute.Tags], Summary = "list Commune")]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(
        ListCommuneQuery request,
        CancellationToken cancellationToken = default
    ) => this.Ok200(await sender.Send(request, cancellationToken));
}
