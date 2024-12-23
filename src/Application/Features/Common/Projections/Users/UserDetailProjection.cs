using Application.Features.Common.Projections.Roles;

namespace Application.Features.Common.Projections.Users;

public class UserDetailProjection : UserProjection
{
    public ICollection<RoleDetailProjection>? Roles { get; set; }

    public ICollection<UserClaimDetailProjection>? UserClaims { get; set; }
}
