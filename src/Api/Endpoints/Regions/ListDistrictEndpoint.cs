using Api.common.Documents;
using Api.common.EndpointConfigurations;
using Api.common.Results;
using Api.common.Routers;
using Application.Features.Common.Projections.Regions;
using Application.Features.Regions.Queries.List.Districts;
using Contracts.ApiWrapper;
using Contracts.Dtos.Responses;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.Regions;

public class ListDistrictEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Router.RegionRoute.Districts, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get list of District",
                Description = "Returns a list of districts in Vietnam.",
                Tags = [new OpenApiTag() { Name = Router.RegionRoute.Tags }],
                Parameters = operation.AddDocs(),
            });
    }

    private async Task<
        Results<Ok<ApiResponse<PaginationResponse<DistrictProjection>>>, ProblemHttpResult>
    > HandleAsync(
        ListDistrictQuery request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return result.ToResult();
    }
}
