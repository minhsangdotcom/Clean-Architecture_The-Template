using Application.Features.Common.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.Features.Users.Queries.Profiles;

public class GetUserProfileMapping : Profile
{
    public GetUserProfileMapping()
    {
        CreateMap<User, GetUserProfileResponse>().IncludeBase<User, UserDetailProjection>();
    }
}
