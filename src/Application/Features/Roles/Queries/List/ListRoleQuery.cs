using Contracts.Dtos.Requests;
using Mediator;

namespace Application.Features.Roles.Queries.List;

public class ListRoleQuery() : QueryParamRequest, IRequest<IEnumerable<ListRoleResponse>>;
