using Domain.Aggregates.Users;

namespace Application.Features.Users.Queries.Profiles;

// public class GetUserProfileMapping : Profile
// {
//     public GetUserProfileMapping()
//     {
//         CreateMap<User, GetUserProfileResponse>().IncludeBase<User, UserDetailProjection>();
//     }
// }


public static class GetUserProfileMapping
{
    public static GetUserProfileResponse ToGetUserProfileResponse(this User user)
    {
        var response = new GetUserProfileResponse();
        response.MappingFrom(user);

        return response;
    }
}
