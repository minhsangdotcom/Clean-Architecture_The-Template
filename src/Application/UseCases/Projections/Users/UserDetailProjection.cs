using Application.UseCases.Projections.Roles;

namespace Application.UseCases.Projections.Users;

public class UserDetailProjection : UserProjection
{
    public IEnumerable<RoleProjection>? Roles { get; set; }

    public IEnumerable<UserClaimDetailProjection>? Claims { get; set; }
}