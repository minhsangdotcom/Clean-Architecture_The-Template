using Mediator;

namespace Application.UseCases.Users.Commands.RequestForgotPassword;

public record RequestForgotUserPasswordCommand(string Email) : IRequest;
