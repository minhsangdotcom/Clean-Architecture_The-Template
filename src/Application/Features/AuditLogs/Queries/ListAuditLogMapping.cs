using Application.UseCases.AuditLogs.Queries;
using AutoMapper;
using Domain.Aggregates.AuditLogs;

namespace Application.Features.AuditLogs.Queries;

public class ListAuditLogMapping : Profile
{
    public ListAuditLogMapping()
    {
        CreateMap<AuditLog, ListAuditlogResponse>();
    }
}
