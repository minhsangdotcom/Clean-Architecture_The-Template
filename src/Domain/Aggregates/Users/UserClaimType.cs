using Domain.Aggregates.Users.Enums;

namespace Domain.Aggregates.Users;

public class UserClaimType
{
    public Ulid? Id { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    public KindaUserClaimType Type { get; set; } = KindaUserClaimType.Default;
}