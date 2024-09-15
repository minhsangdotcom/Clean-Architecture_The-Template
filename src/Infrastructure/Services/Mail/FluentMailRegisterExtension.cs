using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Mail;

public static class FluentMailRegisterExtension
{
    public static IServiceCollection AddFluentMail(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var emailConfig = configuration.GetSection("EmailSettings");
        services.AddOptions<EmailSettings>().Bind(emailConfig);

        EmailSettings emailSettings = emailConfig.Get<EmailSettings>()!;

        var email = emailSettings.From;
        var host = emailSettings.Host;
        var port = emailSettings.Port;
        var userName = emailSettings.Username;
        var password = emailSettings.Password;

        services
            .AddFluentEmail(email)
            .AddRazorRenderer()
            .AddSmtpSender(host, port, userName, password);

        return services;
    }
}
