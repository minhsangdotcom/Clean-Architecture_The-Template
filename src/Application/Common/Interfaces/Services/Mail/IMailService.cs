using Contracts.Dtos.Requests;

namespace Application.Common.Interfaces.Services.Mail;

public interface IMailService : IDisposable
{
    Task<bool> SendAsync(MailMessageData metaData);
    Task<bool> SendWithTemplateAsync(MailTemplateData metaData);
}
