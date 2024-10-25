using Application.Common.Interfaces.Services;
using Application.Common.QueryStringProcessing;
using Application.UseCases.Projections.Regions;
using Contracts.Dtos.Responses;
using Mediator;

namespace Application.UseCases.Regions.Queries.List.Communes;

public class ListCommuneHandler(IRegionService regionService)
    : IRequestHandler<ListCommuneQuery, PaginationResponse<CommuneDetailProjection>>
{
    public async ValueTask<PaginationResponse<CommuneDetailProjection>> Handle(
        ListCommuneQuery request,
        CancellationToken cancellationToken
    ) =>
        await regionService.Communes<CommuneDetailProjection>(
            request.ValidateQuery().ValidateFilter(typeof(CommuneDetailProjection))
        );
}
