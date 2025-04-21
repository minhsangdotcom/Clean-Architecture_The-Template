using Contracts.ApiWrapper;
using Mediator;

namespace Application.Features.Users.Commands.RequestResetPassword;

public record RequestResetUserPasswordCommand(string Email) : IRequest<Result<string>>;
