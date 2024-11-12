using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.QueryStringProcessing;
using Application.UseCases.Projections.Regions;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Regions.Specifications;
using Mediator;

namespace Application.UseCases.Regions.Queries.List.Districts;

public class ListDistrictHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListDistrictQuery, PaginationResponse<DistrictDetailProjection>>
{
    public async ValueTask<PaginationResponse<DistrictDetailProjection>> Handle(
        ListDistrictQuery request,
        CancellationToken cancellationToken
    ) =>
        await unitOfWork
            .Repository<District>()
            .PagedListAsync<DistrictDetailProjection>(
                new ListDistrictSpecification(),
                request.ValidateQuery().ValidateFilter(typeof(DistrictDetailProjection))
            );
}