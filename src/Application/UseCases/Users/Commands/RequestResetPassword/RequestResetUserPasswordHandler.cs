using Application.Common.Events;
using Application.Common.Exceptions;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Common.Messages;
using Contracts.Extensions;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using Microsoft.Extensions.Configuration;

namespace Application.UseCases.Users.Commands.RequestResetPassword;

public class RequestResetUserPasswordHandler(
    IUnitOfWork unitOfWork,
    IPublisher mediator,
    IConfiguration configuration
) : IRequestHandler<RequestResetUserPasswordCommand>
{
    public async ValueTask<Unit> Handle(
        RequestResetUserPasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        User user =
            await unitOfWork
                .CachedRepository<User>()
                .FindByConditionAsync(new GetUserByEmailSpecification(command.Email))
            ?? throw new NotFoundException(
                [Messager.Create<User>().Message(MessageType.Found).Negative().Build()]
            );

        string token = StringExtension.GenerateRandomString(40);
        Console.WriteLine(token +" - " + user.Id);
        
        DateTimeOffset expiredTime = DateTimeOffset.UtcNow.AddHours(
            configuration.GetValue<int>("ForgotPasswordExpiredTimeInHour")
        );
        UserResetPassword userResetPassword =
            new()
            {
                Token = token,
                UserId = user.Id,
                Expiry = expiredTime,
            };

        await unitOfWork.Repository<UserResetPassword>().DeleteRangeAsync(user.UserResetPasswords!);
        await unitOfWork.Repository<UserResetPassword>().AddAsync(userResetPassword);
        await unitOfWork.SaveAsync(cancellationToken);

        string domain = configuration.GetValue<string>("ForgotPassowordUrl")!;
        var link = new UriBuilder(domain) { Query = $"token={token}&id={user.Id}" };
        string expiry = expiredTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss");

        await mediator.Publish(
            new EmailSenderEvent(user.Email, link.ToString(), expiry, "ForgotPassword"),
            cancellationToken
        );
        return Unit.Value;
    }
}
