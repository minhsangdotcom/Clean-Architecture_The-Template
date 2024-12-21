using Application.UseCases.Projections.Roles;

namespace Application.UseCases.Projections.Users;

public class UserDetailProjection : UserProjection
{
    public ICollection<RoleDetailProjection>? Roles { get; set; }

    public ICollection<UserClaimDetailProjection>? UserClaims { get; set; }
}
