using Application.UseCases.Projections.Users;
using Application.UseCases.Users.Queries.Detail;
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
