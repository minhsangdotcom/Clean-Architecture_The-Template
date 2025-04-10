using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.QueryStringProcessing;
using Contracts.ApiWrapper;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;

namespace Application.Features.Users.Queries.List;

public class ListUserHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListUserQuery, Result<PaginationResponse<ListUserResponse>>>
{
    public async ValueTask<Result<PaginationResponse<ListUserResponse>>> Handle(
        ListUserQuery query,
        CancellationToken cancellationToken
    ) =>
        Result<PaginationResponse<ListUserResponse>>.Success(
            await unitOfWork
                .DynamicReadOnlyRepository<User>(true)
                .CursorPagedListAsync(
                    new ListUserSpecification(),
                    query.ValidateQuery().ValidateFilter<ListUserResponse>(),
                    ListUserMapping.Selector(),
                    cancellationToken: cancellationToken
                )
        );
}
