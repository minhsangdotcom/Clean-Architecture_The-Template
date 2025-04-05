using Domain.Aggregates.Users;

namespace Application.Features.Users.Queries.Detail;

// public class GetUserDetailMapping : Profile
// {
//     public GetUserDetailMapping()
//     {
//         CreateMap<User, GetUserDetailResponse>().IncludeBase<User, UserDetailProjection>();
//     }
// }


public static class GetUserDetailMapping
{
    public static GetUserDetailResponse ToGetUserDetailResponse(this User user)
    {
        var response = new GetUserDetailResponse();
        response.MappingFrom(user);

        return response;
    }
}
