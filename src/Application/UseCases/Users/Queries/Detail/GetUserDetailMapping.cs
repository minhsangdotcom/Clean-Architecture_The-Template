using Application.UseCases.Projections.Regions;
using Application.UseCases.Projections.Roles;
using Application.UseCases.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Regions;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.ValueObjects;

namespace Application.UseCases.Users.Queries.Detail;

public class GetUserDetailMapping : Profile
{
    public GetUserDetailMapping()
    {
        CreateMap<User, GetUserDetailResponse>().IncludeBase<User, UserDetailProjection>();
    }
}
