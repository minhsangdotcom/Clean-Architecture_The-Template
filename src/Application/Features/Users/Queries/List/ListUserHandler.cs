using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.QueryStringProcessing;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using SharedKernel.Responses;

namespace Application.Features.Users.Queries.List;

public class ListUserHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListUserQuery, PaginationResponse<ListUserResponse>>
{
    public async ValueTask<PaginationResponse<ListUserResponse>> Handle(
        ListUserQuery query,
        CancellationToken cancellationToken
    ) =>
        await unitOfWork
            .CachedRepository<User>()
            .CursorPagedListAsync<ListUserResponse>(
                new ListUserSpecification(),
                query.ValidateQuery().ValidateFilter(typeof(ListUserResponse))
            );
}
