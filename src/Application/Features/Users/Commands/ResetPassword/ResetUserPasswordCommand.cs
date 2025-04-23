using Contracts.ApiWrapper;
using Mediator;

namespace Application.Features.Users.Commands.ResetPassword;

public record ResetUserPasswordCommand(string Token, Ulid UserId, string Password)
    : IRequest<Result<string>>;
