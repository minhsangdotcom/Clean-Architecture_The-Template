using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.QueryStringProcessing;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;

namespace Application.Features.Users.Queries.List;

public class ListUserHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListUserQuery, PaginationResponse<ListUserResponse>>
{
    public async ValueTask<PaginationResponse<ListUserResponse>> Handle(
        ListUserQuery query,
        CancellationToken cancellationToken
    ) =>
        await unitOfWork
            .ReadOnlyRepository<User>(true)
            .CursorPagedListAsync(
                new ListUserSpecification(),
                query.ValidateQuery().ValidateFilter(typeof(ListUserResponse)),
                ListUserMapping.Selector(),
                cancellationToken: cancellationToken
            );
}
