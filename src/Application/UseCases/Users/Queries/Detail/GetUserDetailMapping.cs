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
        CreateMap<User, GetUserDetailResponse>();

        CreateMap<User, UserDetailProjection>().IncludeMembers(x => x.Address)
            .ForMember(dest => dest.Claims, opt => opt.MapFrom(src => src.UserClaims))
            .ForMember(
                dest => dest.Roles,
                opt => opt.MapFrom(src => src.UserRoles!.Select(x => x.Role))
            )
            .IncludeAllDerived();

        CreateMap<Address, UserDetailProjection>();

        CreateMap<Role, RoleProjection>();
        CreateMap<Role, RoleDetailProjection>();
        CreateMap<UserClaim, UserClaimDetailProjection>();

        CreateMap<Commune, CommuneProjection>();
        CreateMap<District, DistrictProjection>();
        CreateMap<Province, ProvinceProjection>();

        CreateMap<District, DistrictDetailProjection>();
        CreateMap<Province, ProvinceDetailProjection>();
    }
}
