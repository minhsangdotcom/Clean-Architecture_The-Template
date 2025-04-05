using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.QueryStringProcessing;
using Application.Features.Common.Mapping.Regions;
using Application.Features.Common.Projections.Regions;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Regions.Specifications;
using Mediator;

namespace Application.Features.Regions.Queries.List.Communes;

public class ListCommuneHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListCommuneQuery, PaginationResponse<CommuneDetailProjection>>
{
    public async ValueTask<PaginationResponse<CommuneDetailProjection>> Handle(
        ListCommuneQuery request,
        CancellationToken cancellationToken
    ) =>
        await unitOfWork
            .SpecificationRepository<Commune>()
            .PagedListAsync(
                new ListCommuneSpecification(),
                request.ValidateQuery().ValidateFilter(typeof(CommuneDetailProjection)),
                x => x.ToCommuneDetailProjection(),
                cancellationToken
            );
}
