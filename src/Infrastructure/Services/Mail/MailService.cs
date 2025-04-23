using System.Net;
using System.Net.Mail;
using Application.Common.Interfaces.Services.Mail;
using Contracts.Dtos.Requests;
using Microsoft.Extensions.Options;
using Serilog;

namespace Infrastructure.Services.Mail;

public class MailService : IMailService
{
    private readonly SmtpClient smtpClient;
    private readonly ILogger logger;
    private readonly EmailSettings emailSettings;

    private bool disposed;

    public MailService(IOptions<EmailSettings> options, ILogger logger)
    {
        emailSettings = options.Value;
        smtpClient = new(emailSettings.Host, emailSettings.Port)
        {
            UseDefaultCredentials = false,
            EnableSsl = true,
            Credentials = new NetworkCredential(emailSettings.From, emailSettings.Password),
        };
        this.logger = logger;
    }

    public async Task<bool> SendAsync(MailMessageData metaData)
    {
        try
        {
            MailMessage message = CreateEmailMessage(
                new MailData()
                {
                    DisplayName = metaData.DisplayName,
                    Subject = metaData.Subject,
                    To = metaData.To,
                },
                metaData.Message!
            );
            await smtpClient.SendMailAsync(message);
            logger.Information(
                "Email has been sent successfully to {recipients}",
                string.Join(", ", metaData.To)
            );
            return true;
        }
        catch (Exception ex)
        {
            logger.Error(
                ex,
                "Failed to send email to {recipients}: {error}",
                string.Join(", ", metaData.To),
                ex.Message
            );
            return false;
        }
    }

    public async Task<bool> SendWithTemplateAsync(MailTemplateData metaData)
    {
        try
        {
            string template = await RazorViewToStringRenderer.RenderViewToStringAsync(
                metaData.Template!
            );
            MailMessage message = CreateEmailMessage(
                new MailData()
                {
                    DisplayName = metaData.DisplayName,
                    Subject = metaData.Subject,
                    To = metaData.To,
                },
                template
            );
            await smtpClient.SendMailAsync(message);
            logger.Information(
                "Email has been sent successfully to {recipients}",
                string.Join(", ", metaData.To)
            );
            return true;
        }
        catch (Exception ex)
        {
            logger.Error(
                ex,
                "Failed to send email to {recipients}: {error}",
                string.Join(", ", metaData.To),
                ex.Message
            );
            return false;
        }
    }

    private MailMessage CreateEmailMessage(MailData mailData, string body)
    {
        MailMessage mailMessage =
            new()
            {
                Subject = mailData.Subject,
                Body = body,
                From = new MailAddress(emailSettings.From!, mailData.DisplayName),
                IsBodyHtml = true,
            };

        foreach (string recipient in mailData.To)
        {
            mailMessage.To.Add(recipient);
        }

        return mailMessage;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                smtpClient?.Dispose();
            }
            disposed = true;
        }
    }

    // Finalizer in case Dispose is not called
    ~MailService()
    {
        Dispose(false);
    }
}
