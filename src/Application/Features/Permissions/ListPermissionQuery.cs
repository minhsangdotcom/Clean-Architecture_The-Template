using Contracts.ApiWrapper;
using Contracts.Dtos.Requests;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Permissions;

public class ListPermissionQuery
    : QueryParamRequest,
        IRequest<Result<IEnumerable<ListPermissionResponse>>>
{
    public static ValueTask<ListPermissionQuery> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult(QueryParamRequestExtension.Bind<ListPermissionQuery>(context));
    }
}
