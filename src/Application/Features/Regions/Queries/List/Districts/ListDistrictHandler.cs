using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.QueryStringProcessing;
using Application.Features.Common.Projections.Regions;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Regions.Specifications;
using Mediator;

namespace Application.Features.Regions.Queries.List.Districts;

public class ListDistrictHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListDistrictQuery, PaginationResponse<DistrictProjection>>
{
    public async ValueTask<PaginationResponse<DistrictProjection>> Handle(
        ListDistrictQuery request,
        CancellationToken cancellationToken
    ) =>
        await unitOfWork
            .DynamicReadOnlyRepository<District>()
            .PagedListAsync(
                new ListDistrictSpecification(),
                request.ValidateQuery().ValidateFilter<DistrictProjection>(),
                district => district.ToDistrictProjection(),
                cancellationToken
            );
}
