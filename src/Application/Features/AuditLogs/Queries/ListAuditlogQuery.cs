using Contracts.ApiWrapper;
using Contracts.Dtos.Requests;
using Mediator;
using Microsoft.AspNetCore.Http;
using SharedKernel.Models;

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
