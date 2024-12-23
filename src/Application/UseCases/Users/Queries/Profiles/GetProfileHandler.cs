using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;

namespace Application.UseCases.Users.Queries.Profiles;

public class GetProfileHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    : IRequestHandler<GetUserProfileQuery, GetUserProfileResponse>
{
    public async ValueTask<GetUserProfileResponse> Handle(
        GetUserProfileQuery query,
        CancellationToken cancellationToken
    ) =>
        await unitOfWork
            .Repository<User>()
            .FindByConditionAsync<GetUserProfileResponse>(
                new GetUserByIdSpecification(currentUser.Id!.Value),
                cancellationToken
            )
        ?? throw new NotFoundException(
            [Messager.Create<User>().Message(MessageType.Found).Negative().BuildMessage()]
        );
}
