using Application.UseCases.Projections.Roles;
using Application.UseCases.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;

namespace Application.UseCases.Users.Commands.Create;

public class CreateUserMapping : Profile
{
    public CreateUserMapping()
    {
        CreateMap<CreateUserCommand, User>()
            .AfterMap((src, dest) =>
            {
                dest.SetPassword(HashPassword(src.Password));
            });

        CreateMap<UserClaimModel, UserClaimType>()
            .AfterMap((src, dest, context) =>
            {
                if (Enum.TryParse(context.Items[nameof(UserClaimType.Type)]?.ToString(), out KindaUserClaimType type))
                {
                    dest.Type = type;
                }
            });

        CreateMap<User, UserDetailProjection>()
            .ForMember(dest => dest.Claims, opt => opt.MapFrom(src => src.UserClaims))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles!.Select(x => x.Role)))
            .IncludeAllDerived();

        CreateMap<User, CreateUserResponse>();

        CreateMap<Role, RoleProjection>();
        CreateMap<UserClaim, UserClaimDetailProjection>();
    }
}