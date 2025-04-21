using Application.Features.Common.Mapping.Users;
using Application.Features.Common.Projections.Roles;
using Domain.Aggregates.Users;

namespace Application.Features.Common.Projections.Users;

public class UserDetailProjection : UserProjection
{
    public ICollection<RoleDetailProjection>? Roles { get; set; }

    public ICollection<UserClaimDetailProjection>? UserClaims { get; set; }

    public override void MappingFrom(User user)
    {
        base.MappingFrom(user);
        Roles = user
            .UserRoles?.Select(userRole =>
            {
                var userResonse = new RoleDetailProjection();
                userResonse.MappingFrom(userRole.Role!);
                return userResonse;
            })
            .ToList();
        UserClaims = user.UserClaims?.Select(claim => claim.ToRoleClaimDetailProjection()).ToList();
    }
}
