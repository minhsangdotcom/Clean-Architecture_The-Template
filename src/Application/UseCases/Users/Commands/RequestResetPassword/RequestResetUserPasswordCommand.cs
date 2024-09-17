using Mediator;

namespace Application.UseCases.Users.Commands.RequestResetPassword;

public record RequestResetUserPasswordCommand(string Email) : IRequest;
