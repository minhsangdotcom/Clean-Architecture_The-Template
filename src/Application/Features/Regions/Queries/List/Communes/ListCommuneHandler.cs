using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.QueryStringProcessing;
using Application.Features.Common.Mapping.Regions;
using Application.Features.Common.Projections.Regions;
using Contracts.ApiWrapper;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Regions.Specifications;
using Mediator;

namespace Application.Features.Regions.Queries.List.Communes;

public class ListCommuneHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListCommuneQuery, Result<PaginationResponse<CommuneProjection>>>
{
    public async ValueTask<Result<PaginationResponse<CommuneProjection>>> Handle(
        ListCommuneQuery request,
        CancellationToken cancellationToken
    ) =>
        Result<PaginationResponse<CommuneProjection>>.Success(
            await unitOfWork
                .DynamicReadOnlyRepository<Commune>()
                .PagedListAsync(
                    new ListCommuneSpecification(),
                    request.ValidateQuery().ValidateFilter<CommuneProjection>(),
                    commune => commune.ToCommuneProjection(),
                    cancellationToken
                )
        );
}
