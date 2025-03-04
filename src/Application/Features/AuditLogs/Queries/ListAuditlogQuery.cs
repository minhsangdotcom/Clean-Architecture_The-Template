using Application.UseCases.AuditLogs.Queries;
using Mediator;
using SharedKernel.Requests;
using SharedKernel.Responses;

namespace Application.Features.AuditLogs.Queries;

public class ListAuditlogQuery
    : QueryParamRequest,
        IRequest<PaginationResponse<ListAuditlogResponse>>;
