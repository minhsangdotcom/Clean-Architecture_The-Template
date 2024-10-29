using Application.Common.Interfaces.Services.Mail;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Mail;

public class Mailer(IServiceScopeFactory serviceScopeFactory, IOptions<EmailSettings> options)
    : IMailer
{
    public IMailService Email()
    {
        MailType type = options.Value.MailType;

        using var scope = serviceScopeFactory.CreateScope();
        IServiceProvider serviceProvider = scope.ServiceProvider;
        return type switch
        {
            MailType.Kit => serviceProvider.GetRequiredService<KitMailService>(),
            MailType.Fluent => serviceProvider.GetRequiredService<FluentMailService>(),
            _ => throw new ArgumentException("Invalid email provider"),
        };
    }
}
