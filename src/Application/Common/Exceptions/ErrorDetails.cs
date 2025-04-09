using Microsoft.AspNetCore.Http;

namespace Application.Common.Exceptions;

public class ErrorDetails(string title, string message, string? type = null) : Exception(message)
{
    public int Status { get; set; } = StatusCodes.Status500InternalServerError;
    public string? Title { get; set; } = title;
    public string? Detail { get; set; } = message;

    public string? Type { get; set; } = type ?? "InternalException";
}
