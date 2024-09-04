namespace Application.UseCases.Projections.Roles;

public class RoleDetailProjection : RoleProjection
{
    public IEnumerable<RoleClaimDetailProjection>? RoleClaims { get; set; }
}