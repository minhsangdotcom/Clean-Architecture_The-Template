using Contracts.Dtos.Requests;
using Mediator;

namespace Application.Features.Permissions;

public class ListPermissionQuery : QueryParamRequest, IRequest<IEnumerable<ListPermissionResponse>>;
