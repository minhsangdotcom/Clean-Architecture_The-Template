namespace Application.Features.Common.Payloads.Users;

public class UserClaimPayload
{
    public Ulid? Id { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }
}
