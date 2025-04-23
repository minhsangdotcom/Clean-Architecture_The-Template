using Contracts.ApiWrapper;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Users.Queries.List;

public class ListUserQuery
    : QueryParamRequest,
        IRequest<Result<PaginationResponse<ListUserResponse>>>
{
    public static ValueTask<ListUserQuery> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult(QueryParamRequestExtension.Bind<ListUserQuery>(context));
    }
}
