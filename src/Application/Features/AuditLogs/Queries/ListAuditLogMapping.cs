using AutoMapper;
using Domain.Aggregates.AuditLogs;

namespace Application.UseCases.AuditLogs.Queries;

public class ListAuditLogMapping : Profile
{
    public ListAuditLogMapping()
    {
        CreateMap<AuditLog, ListAuditlogResponse>();
    }
}
