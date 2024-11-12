using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.QueryStringProcessing;
using Application.UseCases.Projections.Regions;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Regions.Specifications;
using Mediator;

namespace Application.UseCases.Regions.Queries.List.Provinces;

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
