using Domain.Common;
namespace Domain.Aggregates.Users;

public class Role : DefaultEntity
{
    public string? Guard { get; set; }

    public string? Description { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; set; } = [];

    public ICollection<RoleClaim> RoleClaims { get; set; } = [];
}