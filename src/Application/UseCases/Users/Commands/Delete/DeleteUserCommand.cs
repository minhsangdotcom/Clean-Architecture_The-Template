using Mediator;

namespace Application.UseCases.Users.Commands.Delete;

public record DeleteUserCommand(Ulid UserId) : IRequest;