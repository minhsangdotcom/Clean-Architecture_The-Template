using Application.Common.Interfaces.Services.Elastics;
using Application.UseCases.AuditLogs.Queries;
using Domain.Aggregates.AuditLogs;
using Mediator;
using SharedKernel.Responses;

namespace Application.Features.AuditLogs.Queries;

public class ListAuditlogHandler(IElasticsearchServiceFactory? elasticsearch = null)
    : IRequestHandler<ListAuditlogQuery, PaginationResponse<ListAuditlogResponse>>
{
    public async ValueTask<PaginationResponse<ListAuditlogResponse>> Handle(
        ListAuditlogQuery request,
        CancellationToken cancellationToken
    )
    {
        if (elasticsearch == null)
        {
            throw new NotImplementedException();
        }
        return await elasticsearch
            .Get<AuditLog>()
            .PaginatedListAsync<ListAuditlogResponse>(request);
    }
}
