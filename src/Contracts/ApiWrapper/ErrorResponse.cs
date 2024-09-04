using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Contracts.Extensions;
using Microsoft.AspNetCore.Http;

namespace Contracts.ApiWrapper;

public class ErrorResponse : ApiBaseResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? TraceId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? ResponseException { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ICollection<ValidationError>? ValidationErrors { get; }

    public string Title { get; } = "InternalServerException";

    public ErrorResponse()
    {
    }

    public ErrorResponse(
        string message,
        string? title = null,
        Guid? traceId = null,
        object? responseException = null,
        int? statusCode = StatusCodes.Status500InternalServerError
    )
    {
        StatusCode = statusCode!.Value;
        ResponseException = responseException;
        Message = message;
        TraceId = traceId;
        
        if(!string.IsNullOrWhiteSpace(title))
        {
            Title = title;
        }
    }

    public ErrorResponse(
        ICollection<ValidationError>? validationErrors,
        string message,
        int? statusCode = StatusCodes.Status400BadRequest
    )
    {
        StatusCode = statusCode!.Value;
        ValidationErrors = validationErrors;
        Message = message;
        Title = nameof(ValidationException);
    }

    public override string ToString() =>
        SerializerExtension.Serialize(this);
}