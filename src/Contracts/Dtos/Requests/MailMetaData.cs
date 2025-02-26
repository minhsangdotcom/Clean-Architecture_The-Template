namespace Contracts.Dtos.Requests;

public class MailMetaData
{
    public required string? Subject { get; set; }

    public required string? DisplayName { get; set; }

    public required List<string> To { get; set; } = [];
}

public class MessageMailMetaData : MailMetaData
{
    public required string? Message { get; set; }
}

public class TemplateMailMetaData : MailMetaData
{
    public required MailTemplate? Template { get; set; }
}

public class MailData : MailMetaData
{
    public required string? Body { get; set; }
}

public enum MailType
{
    Kit = 1,
    Fluent = 2,
}

public record MailTemplate(string ViewName, object Template);
