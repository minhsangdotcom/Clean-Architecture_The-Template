using Application.Features.Common.Projections.Users;
using Domain.Aggregates.Users;

namespace Application.Features.Common.Mapping.Users;

// public class UserMapping : Profile
// {
//     public UserMapping()
//     {
//         CreateMap<User, UserProjection>().IncludeMembers(x => x.Address);

//         CreateMap<User, UserDetailProjection>()
//             .IncludeMembers(x => x.Address)
//             .ForMember(
//                 dest => dest.Roles,
//                 opt => opt.MapFrom(src => src.UserRoles!.Select(x => x.Role))
//             );
//         CreateMap<Address, UserProjection>();
//         CreateMap<Address, UserDetailProjection>();

//         CreateMap<Role, RoleDetailProjection>();
//         CreateMap<RoleClaim, RoleClaimDetailProjection>();
//         CreateMap<UserClaim, UserClaimDetailProjection>();

//         CreateMap<UserClaimModel, UserClaim>()
//             .AfterMap(
//                 (src, dest, context) =>
//                 {
//                     if (
//                         Enum.TryParse(
//                             context.Items[nameof(UserClaim.Type)]?.ToString(),
//                             out KindaUserClaimType type
//                         )
//                     )
//                     {
//                         dest.Type = type;
//                     }

//                     if (
//                         Ulid.TryParse(
//                             context.Items[nameof(UserClaim.UserId)]?.ToString(),
//                             out Ulid id
//                         )
//                     )
//                     {
//                         dest.UserId = id;
//                     }

//                     if (src.Id == null || src.Id == Ulid.Empty)
//                     {
//                         dest.Id = Ulid.NewUlid();
//                     }
//                 }
//             );
//     }
// }

public static class UserMapping
{
    public static UserClaimDetailProjection ToRoleClaimDetailProjection(this UserClaim userClaim)
    {
        return new()
        {
            ClaimType = userClaim.ClaimType,
            ClaimValue = userClaim.ClaimValue,
            Id = userClaim.Id,
            CreatedAt = userClaim.CreatedAt,
        };
    }

    public static UserProjection ToUserProjection(this User user)
    {
        var response = new UserProjection();
        response.MappingFrom(user);

        return response;
    }
}
