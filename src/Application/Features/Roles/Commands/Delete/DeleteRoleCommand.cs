using Mediator;

namespace Application.Features.Roles.Commands.Delete;

public record DeleteRoleCommand(Ulid RoleId) : IRequest;
