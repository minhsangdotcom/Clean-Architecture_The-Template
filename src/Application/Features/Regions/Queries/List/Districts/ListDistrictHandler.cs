using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.QueryStringProcessing;
using Application.Features.Common.Projections.Regions;
using Contracts.ApiWrapper;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Regions.Specifications;
using Mediator;

namespace Application.Features.Regions.Queries.List.Districts;

public class ListDistrictHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListDistrictQuery, Result<PaginationResponse<DistrictProjection>>>
{
    public async ValueTask<Result<PaginationResponse<DistrictProjection>>> Handle(
        ListDistrictQuery request,
        CancellationToken cancellationToken
    ) =>
        Result<PaginationResponse<DistrictProjection>>.Success(
            await unitOfWork
                .DynamicReadOnlyRepository<District>()
                .PagedListAsync(
                    new ListDistrictSpecification(),
                    request.ValidateQuery().ValidateFilter<DistrictProjection>(),
                    district => district.ToDistrictProjection(),
                    cancellationToken
                )
        );
}
