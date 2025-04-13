using Microsoft.AspNetCore.Http;
using SharedKernel.Common.Messages;

namespace Contracts.ApiWrapper;

public abstract class ErrorDetails
{
    public int Status { get; set; }
    public string? Title { get; set; }
    public string? Type { get; set; }

    public string? Detail { get; set; }
    public List<InvalidParam>? InvalidParams { get; set; }
    public MessageResult? ErrorMessage { get; set; }

    public ErrorDetails(string title, string detail, string? type = null, int? status = null)
    {
        Title = title;
        Status = status ?? StatusCodes.Status500InternalServerError;
        Detail = detail;
        Type = type ?? "InternalException";
    }

    public ErrorDetails(
        string title,
        List<InvalidParam> invalidParams,
        string? type = null,
        int? status = null
    )
    {
        Title = title;
        Status = status ?? StatusCodes.Status500InternalServerError;
        InvalidParams = invalidParams;
        Type = type ?? "InternalException";
    }

    public ErrorDetails(
        string title,
        MessageResult erorMessage,
        string? type = null,
        int? status = null
    )
    {
        Title = title;
        Status = status ?? StatusCodes.Status500InternalServerError;
        ErrorMessage = erorMessage;
        Type = type ?? "InternalException";
    }
}
