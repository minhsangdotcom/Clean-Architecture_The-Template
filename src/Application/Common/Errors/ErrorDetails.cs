using Contracts.ApiWrapper;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Errors;

public class ErrorDetails : Exception
{
    public int Status { get; set; }
    public string? Title { get; set; }
    public string? Detail { get; set; }

    public string? Type { get; set; }

    public List<InvalidParam>? InvalidParams { get; set; }

    public ErrorDetails(string title, string message, string? type = null, int? status = null)
        : base(message)
    {
        Status = status ?? StatusCodes.Status500InternalServerError;
        Title = title;
        Detail = message;
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
}
