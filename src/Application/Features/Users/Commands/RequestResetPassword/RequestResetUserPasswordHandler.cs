using Application.Common.Exceptions;
using Application.Common.Interfaces.Services.Mail;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Dtos.Models;
using Contracts.Dtos.Requests;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using Microsoft.Extensions.Configuration;
using SharedKernel.Common.Messages;
using SharedKernel.Extensions;

namespace Application.Features.Users.Commands.RequestResetPassword;

public class RequestResetUserPasswordHandler(
    IUnitOfWork unitOfWork,
    IConfiguration configuration,
    IMailService mailService
) : IRequestHandler<RequestResetUserPasswordCommand>
{
    public async ValueTask<Unit> Handle(
        RequestResetUserPasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        User user =
            await unitOfWork
                .DynamicReadOnlyRepository<User>(true)
                .FindByConditionAsync(
                    new GetUserByEmailSpecification(command.Email),
                    cancellationToken
                )
            ?? throw new NotFoundException(
                [Messager.Create<User>().Message(MessageType.Found).Negative().Build()]
            );

        string token = StringExtension.GenerateRandomString(40);
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
        await unitOfWork
            .Repository<UserResetPassword>()
            .AddAsync(userResetPassword, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        string domain = configuration.GetValue<string>("ForgotPassowordUrl")!;
        var link = new UriBuilder(domain) { Query = $"token={token}&id={user.Id}" };
        string expiry = expiredTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss");

        _ = await mailService.SendWithTemplateAsync(
            new MailTemplateData()
            {
                DisplayName = "The template Reset password",
                Subject = "Reset password",
                To = [user.Email],
                Template = new(
                    "ForgotPassword",
                    new ResetPasswordModel() { ResetLink = link.ToString(), Expiry = expiry }
                ),
            }
        );
        return Unit.Value;
    }
}
