using Application.Common.Interfaces.Repositories;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;

namespace Application.UseCases.Users.Queries.List;

public class ListUserHandler(IUnitOfWork unitOfWork) : IRequestHandler<ListUserQuery, PaginationResponse<ListUserResponse>>
{
    public async ValueTask<PaginationResponse<ListUserResponse>> Handle(ListUserQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<User>()
            .PaginatedListSpecificationAsync<ListUserResponse>(new ListUserSpecification(), query);
    }
}