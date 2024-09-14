using Application.Common.Interfaces.Services.Mail;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Mail;

public class Mailer(IServiceProvider serviceProvider)
{
    public IMailService GetEmailService(MailType type)
    {
        using var scope = serviceProvider.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        return type switch
        {
            MailType.Kit => provider.GetRequiredService<KitMailService>(),
            MailType.Fluent => provider.GetRequiredService<FluentMailService>(),
            _ => throw new ArgumentException("Invalid email provider"),
        };
    }
}
