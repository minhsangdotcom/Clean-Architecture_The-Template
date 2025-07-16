using Application.Common.Interfaces.Services.Elastics;
using Contracts.ApiWrapper;
using Domain.Aggregates.AuditLogs;
using Elastic.Clients.Elasticsearch;
using Mediator;
using SharedKernel.Models;

namespace Application.Features.AuditLogs.Queries;

public class ListAuditlogHandler(IElasticsearchServiceFactory? elasticsearch = null)
    : IRequestHandler<ListAuditlogQuery, Result<PaginationResponse<ListAuditlogResponse>>>
{
    public async ValueTask<Result<PaginationResponse<ListAuditlogResponse>>> Handle(
        ListAuditlogQuery request,
        CancellationToken cancellationToken
    )
    {
        if (elasticsearch == null)
        {
            throw new NotImplementedException("Elasticsearch has not enabled");
        }

        SearchResponse<AuditLog> searchResponse = await elasticsearch
            .Get<AuditLog>()
            .ListAsync(request);

        PaginationResponse<ListAuditlogResponse> paginationResponse =
            new(
                searchResponse.Documents.ToListAuditlogResponse(),
                (int)searchResponse.Total,
                request.Page,
                request.PageSize
            );

        return Result<PaginationResponse<ListAuditlogResponse>>.Success(paginationResponse);
    }
}
