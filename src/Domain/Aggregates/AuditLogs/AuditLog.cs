using SharedKernel.Common;

namespace Domain.Aggregates.AuditLogs;

public class AuditLog : DefaultEntity<string>
{
    public string Entity { get; set; } = string.Empty;

    public byte Type { get; set; }

    public object? OldValue { get; set; }

    public object? NewValue { get; set; }

    public string? ActionPerformBy { get; set; }

    public Agent? Agent { get; set; }
}
