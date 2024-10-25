using Domain.Aggregates.Users;
using Domain.Common;
namespace Domain.Aggregates.Roles;

public class Role : DefaultEntity
{
    public string? Guard { get; set; }

    public string? Description { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; set; } = [];

    public ICollection<RoleClaim> RoleClaims { get; set; } = [];
}