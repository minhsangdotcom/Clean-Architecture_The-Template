using Mediator;

namespace Application.Features.Roles.Queries.Detail;

public record GetRoleDetailQuery(Ulid Id) : IRequest<RoleDetailResponse>;
