using Domain.Aggregates.Users.Enums;
using Domain.Common;

namespace Domain.Aggregates.AuditLogs;

public class Agent : DefaultEntity<string>
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }
    
    public DateTime? DayOfBirth { get; set; }

    public byte? Gender { get; set; }

    public RoleTest? Role { get; set; }
}

public class RoleTest
{
    public string? Name { get; set; }

    public string? Guard { get; set; }

    public ICollection<PermissionTest>? Permissions { get; set; }
}

public class PermissionTest
{
    public string? Name { get; set; }
}
