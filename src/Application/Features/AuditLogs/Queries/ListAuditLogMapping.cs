using Domain.Aggregates.AuditLogs;

namespace Application.Features.AuditLogs.Queries;

public static class ListAuditLogMapping
{
    public static IEnumerable<ListAuditlogResponse> ToListAuditlogResponse(
        this IEnumerable<AuditLog> auditLogs
    )
    {
        return auditLogs.Select(auditLog => new ListAuditlogResponse()
        {
            Id = auditLog.Id,
            CreatedAt = auditLog.CreatedAt,
            Entity = auditLog.Entity,
            Agent = auditLog.Agent,
            ActionPerformBy = auditLog.ActionPerformBy,
            NewValue = auditLog.NewValue,
            OldValue = auditLog.OldValue,
            Type = auditLog.Type,
        });
    }
}
