using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.QueryStringProcessing;
using Application.Features.Common.Projections.Regions;
using Contracts.ApiWrapper;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Regions.Specifications;
using Mediator;

namespace Application.Features.Regions.Queries.List.Provinces;

public class ListProvinceHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListProvinceQuery, Result<PaginationResponse<ProvinceProjection>>>
{
    public async ValueTask<Result<PaginationResponse<ProvinceProjection>>> Handle(
        ListProvinceQuery query,
        CancellationToken cancellationToken
    )
    {
        var validationResult = query.ValidateQuery();

        if (validationResult.Error != null)
        {
            return Result<PaginationResponse<ProvinceProjection>>.Failure(validationResult.Error);
        }

        var validationFilterResult = query.ValidateFilter<ListProvinceQuery, ProvinceProjection>();

        if (validationFilterResult.Error != null)
        {
            return Result<PaginationResponse<ProvinceProjection>>.Failure(
                validationFilterResult.Error
            );
        }
        var response = await unitOfWork
            .DynamicReadOnlyRepository<Province>()
            .PagedListAsync(
                new ListProvinceSpecification(),
                query,
                province => province.ToProvinceProjection(),
                cancellationToken
            );
        return Result<PaginationResponse<ProvinceProjection>>.Success(response);
    }
}
