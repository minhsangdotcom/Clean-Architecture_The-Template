using Domain.Common;

namespace Domain.Aggregates.Users;

public class RoleClaim : DefaultEntity
{
    public string ClaimType { get; set; } = string.Empty;

    public string ClaimValue { get; set; } = string.Empty;

    public Role? Role { get; set; }

    public Ulid RoleId { get; set; }

    public ICollection<UserClaim>? UserClaims { get; set; } = [];

    public void UpdateUserClaim()
    {
        if (UserClaims?.Count == 0)
        {
            return;
        }

        UserClaims!.ToList().ForEach(x => x.ClaimValue = ClaimValue);
    }
}
