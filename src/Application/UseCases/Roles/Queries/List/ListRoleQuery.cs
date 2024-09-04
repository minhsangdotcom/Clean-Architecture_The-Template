using Contracts.Dtos.Requests;
using Mediator;
namespace Application.UseCases.Roles.Queries.List;

public class ListRoleQuery() : QueryRequest, IRequest<IEnumerable<ListRoleResponse>>;