using Domain.Common;
namespace Domain.Aggregates.Users;

public class RoleClaim : DefaultEntity
{
    public Role? Role { get; set; }

    public Ulid RoleId { get; set; }

    public string ClaimType { get; set; } = string.Empty;

    public string ClaimValue { get; set; } = string.Empty;
}