using Application.Common.Events;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Contracts.Common.Messages;
using Contracts.Extensions;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using Microsoft.Extensions.Configuration;

namespace Application.UseCases.Users.Commands.RequestForgotPassword;

public class RequestForgotUserPasswordHandler(
    IUnitOfWork unitOfWork,
    IPublisher mediator,
    IConfiguration configuration
) : IRequestHandler<RequestForgotUserPasswordCommand>
{
    public async ValueTask<Unit> Handle(
        RequestForgotUserPasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        User user =
            await unitOfWork
                .Repository<User>()
                .GetByConditionSpecificationAsync(new GetUserByEmailSpecification(command.Email))
            ?? throw new BadRequestException(
                [Messager.Create<User>().Message(MessageType.Found).Negative().Build()]
            );

        string token = StringExtension.GenerateRandomString(40);

        UserResetPassword userResetPassword = new()
        {
            Token = token,
            UserId = user.Id,
            Expiry = DateTimeOffset.UtcNow,
        };

        await unitOfWork.Repository<UserResetPassword>().AddAsync(userResetPassword);
        await unitOfWork.SaveAsync(cancellationToken);

        string domain = configuration.GetValue<string>("ForgotPassowrdUrl")!;
        var link = new UriBuilder(domain)
        {
            Query = $"token={token}&id={user.Id}",
        };

        await mediator.Publish(
            new EmailSenderEvent(user.Email, link.ToString(), "ForgotPassword"),
            cancellationToken
        );
        return Unit.Value;
    }
}
