namespace Contracts.Dtos.Requests;

public class MailMetaData
{
    public string? Subject { get; set; }

    public string? DisplayName { get; set; }

    public List<string> To { get; set; } = [];
}

public class MessageMailMetaData : MailMetaData
{
    public string? Message { get; set; }
}

public class TemplateMailMetaData : MailMetaData
{
    public MailTemplate? Template { get; set; }
}

public class MailData : MailMetaData
{
    public string? Body { get; set; }
}

public record MailTemplate(string ViewName, object Template);
