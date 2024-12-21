using Application.UseCases.Projections.Roles;
using Application.UseCases.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.ValueObjects;

namespace Application.UseCases.Mapping.Users;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserDetailProjection>()
            .IncludeMembers(x => x.Address)
            .ForMember(
                dest => dest.Roles,
                opt => opt.MapFrom(src => src.UserRoles!.Select(x => x.Role))
            );
        CreateMap<Address, UserDetailProjection>();

        CreateMap<Role, RoleDetailProjection>();
        CreateMap<RoleClaim, RoleClaimDetailProjection>();
        CreateMap<UserClaim, UserClaimDetailProjection>();

        CreateMap<UserClaimModel, UserClaim>()
            .AfterMap(
                (src, dest, context) =>
                {
                    if (
                        Enum.TryParse(
                            context.Items[nameof(UserClaim.Type)]?.ToString(),
                            out KindaUserClaimType type
                        )
                    )
                    {
                        dest.Type = type;
                    }

                    if (
                        Ulid.TryParse(
                            context.Items[nameof(UserClaim.UserId)]?.ToString(),
                            out Ulid id
                        )
                    )
                    {
                        dest.UserId = id;
                    }

                    if (src.Id == null || src.Id == Ulid.Empty)
                    {
                        dest.Id = Ulid.NewUlid();
                    }
                }
            );
    }
}
