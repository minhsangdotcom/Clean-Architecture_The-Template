using Application.Common.Interfaces.Services.Mail;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Mail;

public static class MailExtension
{
    public static IServiceCollection AddMail(this IServiceCollection services)
    {
        return services.AddSingleton<IMailService, MailService>();
    }
}
