using Domain.Aggregates.Users.Enums;
using Domain.Common;

namespace Domain.Aggregates.Users;

public class UserClaim : BaseEntity
{
    public User? User { get; set; }

    public Ulid UserId { get; set; }

    public string ClaimType { get; set; } = string.Empty;

    public string ClaimValue { get; set; } = string.Empty;

    public KindaUserClaimType Type { get; set; } = KindaUserClaimType.Default;
}