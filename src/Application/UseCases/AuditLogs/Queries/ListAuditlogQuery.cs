using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;

namespace Application.UseCases.AuditLogs.Queries;

public class ListAuditlogQuery() : QueryRequest, IRequest<PaginationResponse<ListAuditlogResponse>>;
