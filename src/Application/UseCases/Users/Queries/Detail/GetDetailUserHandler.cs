using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;

namespace Application.UseCases.Users.Queries.Detail;

public class GetDetailUserHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDetailUserQuery, GetDetailUserResponse>
{
    public async ValueTask<GetDetailUserResponse> Handle(GetDetailUserQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<User>()
            .GetByConditionSpecificationAsync<GetDetailUserResponse>(new GetUserByIdSpecification(Ulid.Parse(query.UserId))) ??
            throw new BadRequestException($"{nameof(User).ToUpper()}_NOTFOUND");
    }
}