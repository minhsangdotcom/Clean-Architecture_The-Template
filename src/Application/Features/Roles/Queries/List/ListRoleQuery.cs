using Mediator;
using SharedKernel.Requests;

namespace Application.Features.Roles.Queries.List;

public class ListRoleQuery() : QueryParamRequest, IRequest<IEnumerable<ListRoleResponse>>;
