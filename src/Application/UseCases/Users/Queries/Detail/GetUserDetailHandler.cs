using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;

namespace Application.UseCases.Users.Queries.Detail;

public class GetUserDetailHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetUserDetailQuery, GetUserDetailResponse>
{
    public async ValueTask<GetUserDetailResponse> Handle(
        GetUserDetailQuery query,
        CancellationToken cancellationToken
    ) =>
        await unitOfWork
            .Repository<User>()
            .FindByConditionAsync<GetUserDetailResponse>(new GetUserByIdSpecification(query.UserId))
        ?? throw new NotFoundException(
            [Messager.Create<User>().Message(MessageType.Found).Negative().BuildMessage()]
        );
}
