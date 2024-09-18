using Domain.Aggregates.Users.Enums;
using Domain.Common;

namespace Domain.Aggregates.AuditLogs;

public class Agent : DefaultEntity<string>
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }
    
    public DateTime? DayOfBirth { get; set; }

    public Gender? Gender { get; set; }
}
