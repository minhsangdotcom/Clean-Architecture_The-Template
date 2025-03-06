using Mediator;
using SharedKernel.Requests;

namespace Application.Features.Permissions;

public class ListPermissionQuery : QueryParamRequest, IRequest<IEnumerable<ListPermissionResponse>>;
