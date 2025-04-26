using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Api.Extensions;

public static class InfomationLoggingExtension
{
    public static void AddLog(
        this WebApplication webApplication,
        Serilog.ILogger logger,
        string swaggerRoute,
        string healthCheckRoute
    ) =>
        webApplication.Lifetime.ApplicationStarted.Register(() =>
        {
            var server = webApplication.Services.GetRequiredService<IServer>();
            var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses.ToArray();

            if (addresses != null && addresses.Length > 0)
            {
                string? url = addresses?[0];
                logger.Information("Swagger UI is running at: {Url}", $"{url}/{swaggerRoute}");
                logger.Information(
                    "Application health check is running at: {Url}",
                    $"{url}{healthCheckRoute}"
                );
            }
        });
}
