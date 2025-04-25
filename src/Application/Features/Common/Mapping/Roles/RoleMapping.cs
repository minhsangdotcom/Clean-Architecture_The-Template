using Application.Features.Common.Projections.Roles;
using Domain.Aggregates.Roles;

namespace Application.Features.Common.Mapping.Roles;

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
