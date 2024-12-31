using Mediator;

namespace Application.Features.Users.Commands.Delete;

public record DeleteUserCommand(Ulid UserId) : IRequest;
