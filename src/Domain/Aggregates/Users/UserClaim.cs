using Domain.Aggregates.Roles;
using Domain.Aggregates.Users.Enums;
using Domain.Common;

namespace Domain.Aggregates.Users;

public class UserClaim : DefaultEntity
{
    public string ClaimType { get; set; } = string.Empty;

    public string ClaimValue { get; set; } = string.Empty;

    public UserClaimType Type { get; set; } = UserClaimType.Default;

    public User? User { get; set; }

    public Ulid UserId { get; set; }

    public Ulid? RoleClaimId { get; set; }

    public RoleClaim? RoleClaim { get; set; }
}
