using Contracts.Dtos.Requests;
using Mediator;
namespace Application.UseCases.Roles.Queries.List;

public class ListRoleQuery() : QueryParamRequest, IRequest<IEnumerable<ListRoleResponse>>;