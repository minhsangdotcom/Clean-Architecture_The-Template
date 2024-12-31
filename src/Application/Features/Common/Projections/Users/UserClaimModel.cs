namespace Application.Features.Common.Projections.Users;

public class UserClaimModel
{
    public Ulid? Id { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }
}
