using Application.Features.Common.Projections.Regions;
using Contracts.ApiWrapper;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Regions.Queries.List.Provinces;

public class ListProvinceQuery
    : QueryParamRequest,
        IRequest<Result<PaginationResponse<ProvinceProjection>>>
{
    public static ValueTask<ListProvinceQuery> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult(QueryParamRequestExtension.Bind<ListProvinceQuery>(context));
    }
}
