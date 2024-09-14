using Contracts.Dtos.Requests;

namespace Application.Common.Interfaces.Services.Mail;

public interface IMailService
{
    Task<bool> SendAsync(MessageMailMetaData metaData);
    Task<bool> SendWithTemplateAsync(TemplateMailMetaData metaData);
}
