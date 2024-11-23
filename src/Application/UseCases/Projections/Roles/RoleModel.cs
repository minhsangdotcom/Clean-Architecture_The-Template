namespace Application.UseCases.Projections.Roles;

public class RoleModel
{
    public string? Description { get; set; }

    public string? Name { get; set; }

    public List<RoleClaimModel>? RoleClaims { get; set; }
}