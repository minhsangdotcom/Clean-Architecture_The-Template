using Application.UseCases.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.UseCases.Users.Queries.Profiles;

public class GetUserProfileMapping : Profile
{
    public GetUserProfileMapping()
    {
        CreateMap<User, GetUserProfileResponse>().IncludeBase<User, UserDetailProjection>();
    }
}
