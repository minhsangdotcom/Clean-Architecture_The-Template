namespace Application.Features.Common.Payloads.Roles;

public class RoleClaimPayload
{
    public Ulid? Id { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }
}
