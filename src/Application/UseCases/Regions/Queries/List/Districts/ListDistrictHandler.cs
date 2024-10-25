using Application.Common.Interfaces.Services;
using Application.Common.QueryStringProcessing;
using Application.UseCases.Projections.Regions;
using Contracts.Dtos.Responses;
using Mediator;

namespace Application.UseCases.Regions.Queries.List.Districts;

public class ListDistrictHandler(IRegionService regionService)
    : IRequestHandler<ListDistrictQuery, PaginationResponse<DistrictDetailProjection>>
{
    public async ValueTask<PaginationResponse<DistrictDetailProjection>> Handle(
        ListDistrictQuery request,
        CancellationToken cancellationToken
    ) =>
        await regionService.Districts<DistrictDetailProjection>(
            request.ValidateQuery().ValidateFilter(typeof(DistrictDetailProjection))
        );
}
