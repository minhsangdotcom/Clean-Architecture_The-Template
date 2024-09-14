using Application.Common.Interfaces.Services.Mail;
using Contracts.Dtos.Requests;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Mail;

public class FluentMailService(IFluentEmail fluentEmail, ILogger<FluentMailService> logger)
    : IMailService
{
    public async Task<bool> SendAsync(MessageMailMetaData metaData)
    {
        try
        {
            SendResponse res = await fluentEmail
                .To(metaData.To.Select(x => new Address(x, "")))
                .Subject(metaData.Subject)
                .Body(metaData.Message)
                .SendAsync();

            logger.LogInformation("email has been sent successfully.");
            return res.Successful;
        }
        catch (Exception ex)
        {
            logger.LogError("email has been sent fail with error :{error}", ex.Message);
            return false;
        }
    }

    public async Task<bool> SendWithTemplateAsync(TemplateMailMetaData metaData)
    {
        try
        {
            SendResponse res = await fluentEmail
                .To(metaData.To.Select(x => new Address(x, string.Empty)))
                .Subject(metaData.Subject)
                .UsingTemplateFromFile(
                    RazorViewToStringRenderer.GetPath(metaData.Template!.ViewName),
                    metaData.Template!.Template
                )
                .SendAsync();

            logger.LogInformation("email has been sent successfully.");
            return res.Successful;
        }
        catch (Exception ex)
        {
            logger.LogError("email has been sent fail with error :{error}", ex.Message);
            return false;
        }
    }
}
