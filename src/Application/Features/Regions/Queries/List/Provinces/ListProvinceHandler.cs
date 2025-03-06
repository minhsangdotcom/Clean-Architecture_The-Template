using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.QueryStringProcessing;
using Application.Features.Common.Projections.Regions;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Regions.Specifications;
using Mediator;
using SharedKernel.Responses;

namespace Application.Features.Regions.Queries.List.Provinces;

public class ListProvinceHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListProvinceQuery, PaginationResponse<ProvinceProjection>>
{
    public async ValueTask<PaginationResponse<ProvinceProjection>> Handle(
        ListProvinceQuery request,
        CancellationToken cancellationToken
    ) =>
        await unitOfWork
            .Repository<Province>()
            .PagedListAsync<ProvinceProjection>(
                new ListProvinceSpecification(),
                request.ValidateQuery().ValidateFilter(typeof(ProvinceProjection))
            );
}
