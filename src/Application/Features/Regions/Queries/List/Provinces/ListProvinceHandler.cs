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
        ListProvinceQuery request,
        CancellationToken cancellationToken
    ) =>
        Result<PaginationResponse<ProvinceProjection>>.Success(
            await unitOfWork
                .DynamicReadOnlyRepository<Province>()
                .PagedListAsync(
                    new ListProvinceSpecification(),
                    request.ValidateQuery().ValidateFilter<ProvinceProjection>(),
                    province => province.ToProvinceProjection(),
                    cancellationToken
                )
        );
}
