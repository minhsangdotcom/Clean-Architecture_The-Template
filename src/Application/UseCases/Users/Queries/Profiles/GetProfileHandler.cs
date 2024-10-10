using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.UseCases.Users.Queries.Detail;
using AutoMapper;
using Contracts.Common.Messages;
using Domain.Aggregates.Users;
using Mediator;

namespace Application.UseCases.Users.Queries.Profiles;

public class GetProfileHandler(ISender sender, ICurrentUser currentUser, IMapper mapper)
    : IRequestHandler<GetUserProfileQuery, GetUserProfileResponse>
{
    public async ValueTask<GetUserProfileResponse> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken
    ) =>
        mapper.Map<GetUserProfileResponse>(
            await sender.Send(
                new GetUserDetailQuery(currentUser.Id ?? Ulid.Empty),
                cancellationToken
            )
                ?? throw new NotFoundException(
                    [Messager.Create<User>().Message(MessageType.Found).Negative().BuildMessage()]
                )
        );
}
