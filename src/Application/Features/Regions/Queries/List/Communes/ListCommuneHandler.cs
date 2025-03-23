using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.QueryStringProcessing;
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
            .Repository<Commune>()
            .PagedListAsync<CommuneDetailProjection>(
                new ListCommuneSpecification(),
                request.ValidateQuery().ValidateFilter(typeof(CommuneDetailProjection)),
                cancellationToken
            );
}
