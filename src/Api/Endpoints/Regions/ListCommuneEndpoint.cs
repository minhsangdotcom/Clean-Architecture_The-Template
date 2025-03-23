using Api.common.RouteResults;
using Api.common.Routers;
using Application.Features.Common.Projections.Regions;
using Application.Features.Regions.Queries.List.Communes;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Dtos.Responses;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.Regions;

public class ListCommuneEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<ListCommuneQuery>.WithActionResult<
        ApiResponse<PaginationResponse<CommuneDetailProjection>>
    >
{
    [HttpGet(Router.RegionRoute.Communes)]
    [SwaggerOperation(Tags = [Router.RegionRoute.Tags], Summary = "list Commune")]
    public override async Task<
        ActionResult<ApiResponse<PaginationResponse<CommuneDetailProjection>>>
    > HandleAsync(ListCommuneQuery request, CancellationToken cancellationToken = default) =>
        this.Ok200(await sender.Send(request, cancellationToken));
}
