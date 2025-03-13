using Application.Common.Interfaces.Services.Elastics;
using Contracts.Dtos.Responses;
using Domain.Aggregates.AuditLogs;
using Mediator;

namespace Application.Features.AuditLogs.Queries;

public class ListAuditlogHandler(IElasticsearchServiceFactory elasticsearch)
    : IRequestHandler<ListAuditlogQuery, PaginationResponse<ListAuditlogResponse>>
{
    public async ValueTask<PaginationResponse<ListAuditlogResponse>> Handle(
        ListAuditlogQuery request,
        CancellationToken cancellationToken
    )
    {
        return await elasticsearch
            .Get<AuditLog>()
            .PaginatedListAsync<ListAuditlogResponse>(request);
    }
}
