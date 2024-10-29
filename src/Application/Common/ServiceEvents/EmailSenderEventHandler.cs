using Application.Common.Interfaces.Services.Mail;
using Contracts.Dtos.Models;
using Contracts.Dtos.Requests;
using Mediator;

namespace Application.Common.ServiceEvents;

public class EmailSenderEventHandler(IMailer mailer) : INotificationHandler<EmailSenderEvent>
{
    public async ValueTask Handle(
        EmailSenderEvent notification,
        CancellationToken cancellationToken
    ) =>
        await mailer
            .Email()
            .SendWithTemplateAsync(
                new TemplateMailMetaData()
                {
                    DisplayName = "The template Reset password",
                    Subject = "Reset password",
                    To = [notification.Email],
                    Template = new(
                        notification.Template!,
                        new ResetPasswordModel()
                        {
                            ResetLink = notification.ResetLink,
                            Expiry = notification.Expiry,
                        }
                    ),
                }
            );
}
