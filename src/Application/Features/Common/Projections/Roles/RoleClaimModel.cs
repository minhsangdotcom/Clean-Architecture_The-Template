namespace Application.Features.Common.Projections.Roles;

public class RoleClaimModel
{
    public Ulid? Id { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }
}
