using Application.UseCases.Projections.Roles;
using Application.UseCases.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.UseCases.Users.Commands.Create;

public class CreateUserMapping : Profile
{
    public CreateUserMapping()
    {
        CreateMap<CreateUserCommand, User>()
            .AfterMap((src,dest) =>
            {
                dest.SetPassword(HashPassword(src.Password));
            });
        CreateMap<UserClaimModel, UserClaimType>()
            .ForMember(dest => dest.Type , opt => opt.MapFrom((src,_,_,context) => context.Items[nameof(UserClaimType.Type)]));

        CreateMap<User, CreateUserResponse>()
            .ForMember(dest => dest.Claims, opt => opt.MapFrom(src => src.UserClaims))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles!.Select(x => x.Role)));

        CreateMap<Role, RoleProjection>();
        CreateMap<UserClaim, UserClaimDetailProjection>();
    }
}