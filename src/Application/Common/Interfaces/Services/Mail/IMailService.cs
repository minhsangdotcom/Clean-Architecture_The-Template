using Application.Common.Interfaces.Registers;
using Contracts.Dtos.Requests;

namespace Application.Common.Interfaces.Services.Mail;

public interface IMailService : ISingleton, IDisposable
{
    Task<bool> SendAsync(MailMessageData metaData);
    Task<bool> SendWithTemplateAsync(MailTemplateData metaData);
}
