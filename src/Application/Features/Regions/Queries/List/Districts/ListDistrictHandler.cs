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
        ListDistrictQuery query,
        CancellationToken cancellationToken
    )
    {
        var validationResult = query.ValidateQuery();

        if (validationResult.Error != null)
        {
            return Result<PaginationResponse<DistrictProjection>>.Failure(validationResult.Error);
        }

        var validationFilterResult = query.ValidateFilter<ListDistrictQuery, DistrictProjection>();

        if (validationFilterResult.Error != null)
        {
            return Result<PaginationResponse<DistrictProjection>>.Failure(
                validationFilterResult.Error
            );
        }
        var response = await unitOfWork
            .DynamicReadOnlyRepository<District>()
            .PagedListAsync(
                new ListDistrictSpecification(),
                query,
                district => district.ToDistrictProjection(),
                cancellationToken
            );
        return Result<PaginationResponse<DistrictProjection>>.Success(response);
    }
}
