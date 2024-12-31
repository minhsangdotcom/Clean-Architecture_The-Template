namespace Application.Features.Common.Projections.Roles;

public class RoleDetailProjection : RoleProjection
{
    public ICollection<RoleClaimDetailProjection>? RoleClaims { get; set; }
}
