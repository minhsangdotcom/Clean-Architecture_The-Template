using Application.Features.Common.Payloads.Roles;
using Application.Features.Common.Projections.Roles;
using Domain.Aggregates.Roles;

namespace Application.Features.Common.Mapping.Roles;

public static class RoleMapping
{
    public static List<RoleClaim>? ToListRoleClaim(this List<RoleClaimPayload>? roleClaims) =>
        roleClaims?.Select(ToRoleClaim).ToList();

    public static RoleClaim ToRoleClaim(this RoleClaimPayload roleClaim)
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
