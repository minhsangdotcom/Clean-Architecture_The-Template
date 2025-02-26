using Application.Common.Interfaces.Services.Mail;
using AutoMapper;
using Contracts.Dtos.Requests;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Serilog;

namespace Infrastructure.Services.Mail;

public class KitMailService(IOptions<EmailSettings> options, ILogger logger, IMapper mapper)
    : IMailService
{
    private readonly EmailSettings emailSettings = options.Value;

    public async Task<bool> SendAsync(MessageMailMetaData metaData) =>
        await EmailAsync(mapper.Map<MailData>(metaData));

    public async Task<bool> SendWithTemplateAsync(TemplateMailMetaData metaData)
    {
        string body = await RazorViewToStringRenderer.RenderViewToStringAsync(metaData.Template!);
        return await EmailAsync(metaData.MapToMailData(body));
    }

    private async Task<bool> EmailAsync(MailData mailData)
    {
        var emailMessage = CreateEmailMessage(mailData);

        using var client = new SmtpClient();
        try
        {
            await ConnectAndAuthenticateClientAsync(client);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);

            logger.Information(
                "Email has been sent successfully to {recipients}",
                string.Join(", ", mailData.To)
            );
            return true;
        }
        catch (Exception ex)
        {
            logger.Error(
                ex,
                "Failed to send email to {recipients}: {error}",
                string.Join(", ", mailData.To),
                ex.Message
            );
            return false;
        }
    }

    private MimeMessage CreateEmailMessage(MailData mailData)
    {
        var emailMessage = new MimeMessage
        {
            Subject = mailData.Subject,
            Body = new BodyBuilder { HtmlBody = mailData.Body }.ToMessageBody(),
        };

        emailMessage.From.Add(new MailboxAddress(mailData.DisplayName, emailSettings.From));
        emailMessage.To.AddRange(mailData.To.Select(to => new MailboxAddress(string.Empty, to)));

        return emailMessage;
    }

    private async Task ConnectAndAuthenticateClientAsync(SmtpClient client)
    {
        try
        {
            await client.ConnectAsync(
                emailSettings.Host,
                emailSettings.Port,
                SecureSocketOptions.StartTls
            );
            await client.AuthenticateAsync(emailSettings.Username, emailSettings.Password);
        }
        catch (Exception ex)
        {
            await client.DisconnectAsync(true);
            throw new InvalidOperationException(
                "Failed to connect and authenticate SMTP client.",
                ex
            );
        }
    }
}
