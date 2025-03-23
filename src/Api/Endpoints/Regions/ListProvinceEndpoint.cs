using Api.common.RouteResults;
using Api.common.Routers;
using Application.Features.Common.Projections.Regions;
using Application.Features.Regions.Queries.List.Provinces;
using Ardalis.ApiEndpoints;
using Contracts.ApiWrapper;
using Contracts.Dtos.Responses;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.Regions;

public class ListProvinceEndpoint(ISender sender)
    : EndpointBaseAsync.WithRequest<ListProvinceQuery>.WithActionResult<
        ApiResponse<PaginationResponse<ProvinceProjection>>
    >
{
    [HttpGet(Router.RegionRoute.Provinces)]
    [SwaggerOperation(Tags = [Router.RegionRoute.Tags], Summary = "list Province")]
    public override async Task<
        ActionResult<ApiResponse<PaginationResponse<ProvinceProjection>>>
    > HandleAsync(ListProvinceQuery request, CancellationToken cancellationToken = default) =>
        this.Ok200(await sender.Send(request, cancellationToken));
}
