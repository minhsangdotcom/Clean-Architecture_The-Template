using Application.UseCases.Users.Queries.Detail;
using AutoMapper;

namespace Application.UseCases.Users.Queries.Profiles;

public class GetUserProfileMapping : Profile
{
    public GetUserProfileMapping()
    {
        CreateMap<GetUserProfileResponse, GetUserDetailResponse>();
    }
}
