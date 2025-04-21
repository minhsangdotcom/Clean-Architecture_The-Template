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
        ListCommuneQuery query,
        CancellationToken cancellationToken
    )
    {
        var validationResult = query.ValidateQuery();

        if (validationResult.Error != null)
        {
            return Result<PaginationResponse<CommuneProjection>>.Failure(validationResult.Error);
        }

        var validationFilterResult = query.ValidateFilter<ListCommuneQuery, CommuneProjection>();

        if (validationFilterResult.Error != null)
        {
            return Result<PaginationResponse<CommuneProjection>>.Failure(
                validationFilterResult.Error
            );
        }

        var response = await unitOfWork
            .DynamicReadOnlyRepository<Commune>()
            .PagedListAsync(
                new ListCommuneSpecification(),
                query,
                commune => commune.ToCommuneProjection(),
                cancellationToken
            );

        return Result<PaginationResponse<CommuneProjection>>.Success(response);
    }
}
