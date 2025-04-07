using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.QueryStringProcessing;
using Application.Features.Common.Projections.Regions;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Regions.Specifications;
using Mediator;

namespace Application.Features.Regions.Queries.List.Provinces;

public class ListProvinceHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListProvinceQuery, PaginationResponse<ProvinceProjection>>
{
    public async ValueTask<PaginationResponse<ProvinceProjection>> Handle(
        ListProvinceQuery request,
        CancellationToken cancellationToken
    ) =>
        await unitOfWork
            .DynamicRepository<Province>()
            .PagedListAsync(
                new ListProvinceSpecification(),
                request.ValidateQuery().ValidateFilter(typeof(ProvinceProjection)),
                x => x.ToProvinceProjection(),
                cancellationToken
            );
}
