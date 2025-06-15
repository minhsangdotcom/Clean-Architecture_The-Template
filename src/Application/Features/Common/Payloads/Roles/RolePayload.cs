using Application.Features.Common.Projections.Roles;

namespace Application.Features.Common.Payloads.Roles;

public class RolePayload
{
    public string? Description { get; set; }

    public required string? Name { get; set; }

    public List<RoleClaimPayload>? RoleClaims { get; set; }
}
