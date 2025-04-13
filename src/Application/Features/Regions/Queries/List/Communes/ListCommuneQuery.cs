using Application.Features.Common.Projections.Regions;
using Application.Features.Regions.Queries.List.Districts;
using Contracts.ApiWrapper;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Regions.Queries.List.Communes;

public class ListCommuneQuery
    : QueryParamRequest,
        IRequest<Result<PaginationResponse<CommuneProjection>>>
{
    public static ValueTask<ListCommuneQuery> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult(QueryParamRequestExtension.Bind<ListCommuneQuery>(context));
    }
}
