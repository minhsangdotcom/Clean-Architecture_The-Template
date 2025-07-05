using Application.Common.Interfaces.Services.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Mail;

public static class MailExtension
{
    public static IServiceCollection AddMail(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddOptions<EmailSettings>().Bind(configuration.GetSection(nameof(EmailSettings)));
        services.AddScoped<IMailService, MailService>().AddSingleton<RazorViewToStringRenderer>();

        return services;
    }
}
