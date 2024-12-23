using Application.Features.Common.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.Features.Users.Queries.Detail;

public class GetUserDetailMapping : Profile
{
    public GetUserDetailMapping()
    {
        CreateMap<User, GetUserDetailResponse>().IncludeBase<User, UserDetailProjection>();
    }
}
