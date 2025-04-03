using Application.Features.Common.Projections.Roles;
using Domain.Aggregates.Roles;

namespace Application.Features.Common.Mapping.Roles;

// public class RoleMapping : Profile
// {
//     public RoleMapping()
//     {
//         CreateMap<RoleModel, Role>()
//             .ForMember(
//                 dest => dest.Name,
//                 opt => opt.MapFrom(src => src.Name.ToSnakeCase().ToUpper())
//             );

//         CreateMap<RoleClaimModel, RoleClaim>()
//             .ForMember(dest => dest.Id, opt => opt.Ignore())
//             .AfterMap(
//                 (src, dest) =>
//                 {
//                     if (src.Id != null)
//                     {
//                         dest.Id = src.Id.Value;
//                     }
//                 }
//             );
//         CreateMap<RoleClaim, RoleClaimDetailProjection>();
//     }
// }

public static class RoleMapping
{
    public static List<RoleClaim>? ToListRoleClaim(this List<RoleClaimModel>? roleClaims) =>
        roleClaims?.Select(ToRoleClaim).ToList();

    public static RoleClaim ToRoleClaim(this RoleClaimModel roleClaim)
    {
        RoleClaim claim =
            new() { ClaimType = roleClaim.ClaimType!, ClaimValue = roleClaim.ClaimValue! };

        if (roleClaim.Id != null)
        {
            claim.Id = roleClaim.Id!.Value;
        }

        return claim;
    }

    public static ICollection<RoleClaimDetailProjection>? ToListRoleClaimDetailProjection(
        this ICollection<RoleClaim> roleClaims
    ) =>
        roleClaims
            ?.Select(x => new RoleClaimDetailProjection()
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                ClaimType = x.ClaimType,
                ClaimValue = x.ClaimValue,
            })
            .ToArray();
}
