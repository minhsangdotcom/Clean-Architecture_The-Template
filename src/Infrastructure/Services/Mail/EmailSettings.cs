using Contracts.Dtos.Requests;

namespace Infrastructure.Services.Mail;

public class EmailSettings
{
    public string? From { get; set; }

    public string? Host { get; set; }

    public int Port { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public MailType MailType { get; set; }
}
