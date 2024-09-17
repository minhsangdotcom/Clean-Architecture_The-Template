using Mediator;

namespace Application.UseCases.Users.Commands.ResetPassword;

public record ResetUserPasswordCommand(string Token, Ulid UserId, string Password) : IRequest;
