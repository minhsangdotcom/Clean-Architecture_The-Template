using Application.Common.Interfaces.Services;
using Application.UseCases.Users.Queries.Detail;
using AutoMapper;
using Mediator;

namespace Application.UseCases.Users.Queries.Profiles;

public class GetProfileHandler(ISender sender,ICurrentUser currentUser, IMapper mapper) : IRequestHandler<GetUserProfileQuery, GetUserProfileResponse>
{
    public async ValueTask<GetUserProfileResponse> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        return mapper.Map<GetUserProfileResponse>(
            await sender.Send(new GetUserDetailQuery(currentUser.Id ?? Ulid.Empty),cancellationToken)
        );
    }
}