using Application.Features.Common.Projections.Roles;
using Application.Features.Common.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.ValueObjects;

namespace Application.Features.Common.Mapping.Users;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserProjection>().IncludeMembers(x => x.Address);

        CreateMap<User, UserDetailProjection>()
            .IncludeMembers(x => x.Address)
            .ForMember(
                dest => dest.Roles,
                opt => opt.MapFrom(src => src.UserRoles!.Select(x => x.Role))
            );
        CreateMap<Address, UserProjection>();
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
