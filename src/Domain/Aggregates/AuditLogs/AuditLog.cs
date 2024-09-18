using Domain.Aggregates.AuditLogs.Enums;
using Domain.Common;

namespace Domain.Aggregates.AuditLogs;

public class AuditLog : DefaultEntity<string>
{
    public string Entity { get; set; } = string.Empty;

    public AuditLogType Type { get; set; }

    public object? OldValue { get; set; }

    public object? NewValue { get; set; }

    public string? ActionPerformBy { get; set; }

    public Agent? Agent { get; set; }
}
