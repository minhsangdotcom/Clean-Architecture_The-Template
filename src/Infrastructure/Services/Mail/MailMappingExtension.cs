using Contracts.Dtos.Requests;

namespace Infrastructure.Services.Mail;

public static class MailMapping
{
    public static MailData MapToMailData(this MessageMailMetaData messageMailMetaData)
    {
        return new MailData()
        {
            Subject = messageMailMetaData.Subject,
            DisplayName = messageMailMetaData.DisplayName,
            To = messageMailMetaData.To,
            Body = messageMailMetaData.Message,
        };
    }

    public static MailData MapToMailData(this TemplateMailMetaData messageMailMetaData, string body)
    {
        return new MailData()
        {
            Subject = messageMailMetaData.Subject,
            DisplayName = messageMailMetaData.DisplayName,
            To = messageMailMetaData.To,
            Body = body,
        };
    }
}
