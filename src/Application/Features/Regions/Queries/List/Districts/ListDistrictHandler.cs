using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.QueryStringProcessing;
using Application.Features.Common.Projections.Regions;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Regions.Specifications;
using Mediator;

namespace Application.Features.Regions.Queries.List.Districts;

public class ListDistrictHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListDistrictQuery, PaginationResponse<DistrictDetailProjection>>
{
    public async ValueTask<PaginationResponse<DistrictDetailProjection>> Handle(
        ListDistrictQuery request,
        CancellationToken cancellationToken
    ) =>
        await unitOfWork
            .DynamicRepository<District>()
            .PagedListAsync(
                new ListDistrictSpecification(),
                request.ValidateQuery().ValidateFilter(typeof(DistrictDetailProjection)),
                x => x.ToDistrictDetailProjection(),
                cancellationToken
            );
}
