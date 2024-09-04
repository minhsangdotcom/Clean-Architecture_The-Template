using Mediator;
namespace Application.UseCases.Roles.Queries.Detail;

public record GetRoleDetailQuery(Ulid Id) : IRequest<RoleDetailResponse>;