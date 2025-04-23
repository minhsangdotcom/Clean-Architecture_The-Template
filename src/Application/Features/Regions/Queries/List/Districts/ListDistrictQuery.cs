using Application.Features.Common.Projections.Regions;
using Contracts.ApiWrapper;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Regions.Queries.List.Districts;

public class ListDistrictQuery
    : QueryParamRequest,
        IRequest<Result<PaginationResponse<DistrictProjection>>>
{
    public static ValueTask<ListDistrictQuery> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult(QueryParamRequestExtension.Bind<ListDistrictQuery>(context));
    }
}
