using Application.Common.Interfaces.Services.Elastics;
using Contracts.Dtos.Responses;
using Domain.Aggregates.AuditLogs;
using Mediator;

namespace Application.UseCases.AuditLogs.Queries;

public class ListAuditlogHandler(IElasticsearchServiceFactory elasticsearch)
    : IRequestHandler<ListAuditlogQuery, PaginationResponse<ListAuditlogResponse>>
{
    public async ValueTask<PaginationResponse<ListAuditlogResponse>> Handle(
        ListAuditlogQuery request,
        CancellationToken cancellationToken
    )
    {
        var a = await elasticsearch.Get<AuditLog>().PaginatedListAsync<ListAuditlogResponse>(request);
        return a;
    }
}
