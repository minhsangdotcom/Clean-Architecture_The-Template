using Mediator;

namespace Application.UseCases.Roles.Commands.Delete;

public record DeleteRoleCommand(Ulid RoleId) : IRequest;