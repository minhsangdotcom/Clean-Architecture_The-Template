using Contracts.ApiWrapper;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Application.Features.AuditLogs.Queries;

public class ListAuditlogQuery
    : QueryParamRequest,
        IRequest<Result<PaginationResponse<ListAuditlogResponse>>>
{
    public static ValueTask<ListAuditlogQuery> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult(QueryParamRequestExtension.Bind<ListAuditlogQuery>(context));
    }
}
