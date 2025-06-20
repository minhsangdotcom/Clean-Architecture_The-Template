using Application.Common.Errors;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.ApiWrapper;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using SharedKernel.Common.Messages;

namespace Application.Features.Users.Queries.Detail;

public class GetUserDetailHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetUserDetailQuery, Result<GetUserDetailResponse>>
{
    public async ValueTask<Result<GetUserDetailResponse>> Handle(
        GetUserDetailQuery query,
        CancellationToken cancellationToken
    )
    {
        GetUserDetailResponse? user = await unitOfWork
            .DynamicReadOnlyRepository<User>()
            .FindByConditionAsync(
                new GetUserByIdSpecification(query.UserId),
                x => x.ToGetUserDetailResponse(),
                cancellationToken
            );

        if (user == null)
        {
            return Result<GetUserDetailResponse>.Failure(
                new NotFoundError(
                    "",
                    Messenger.Create<User>().Message(MessageType.Found).Negative().BuildMessage()
                )
            );
        }

        return Result<GetUserDetailResponse>.Success(user);
    }
}
