using Contracts.Dtos.Requests;
using RazorLight;

namespace Infrastructure.Services.Mail;

public class RazorViewToStringRenderer
{
    public static async Task<string> RenderViewToStringAsync(MailTemplate mailTemplate)
    {
        RazorLightEngine razorEngine = CreateRazorlight(mailTemplate.Template.GetType());

        try
        {
            var template = await File.ReadAllTextAsync(GetPath(mailTemplate.ViewName));
            return await razorEngine.CompileRenderStringAsync(
                mailTemplate.ViewName,
                template,
                mailTemplate.Template
            );
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to render view {mailTemplate.ViewName}. Exception: {ex.Message}"
            );
        }
    }

    public static string GetPath(string viewName)
    {
        string root = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.FullName;
        return Path.Combine(root, "public", "Templates", $"{viewName}.cshtml");
    }

    private static RazorLightEngine CreateRazorlight(Type type) =>
        new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(type)
            .UseMemoryCachingProvider()
            .Build();
}
