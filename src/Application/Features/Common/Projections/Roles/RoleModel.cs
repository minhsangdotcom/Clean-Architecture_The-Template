namespace Application.Features.Common.Projections.Roles;

public class RoleModel
{
    public required string? Description { get; set; }

    public required string? Name { get; set; }

    public List<RoleClaimModel>? RoleClaims { get; set; }
}
