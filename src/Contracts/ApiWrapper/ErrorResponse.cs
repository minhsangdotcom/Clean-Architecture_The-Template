using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Contracts.Extensions;
using Microsoft.AspNetCore.Http;

namespace Contracts.ApiWrapper;

public class ErrorResponse : ApiBaseResponse
{
    public string Type { get; } = "InternalServerException";

    public Guid? TraceId { get; set; }

    public object? Exception { get; set; }

    public ICollection<BadRequestError>? Errors { get; }

    public ErrorResponse(
        string message,
        string? type = null,
        Guid? traceId = null,
        object? exception = null,
        int? statusCode = StatusCodes.Status500InternalServerError
    )
    {
        StatusCode = statusCode!.Value;
        Exception = exception;
        Message = message;
        TraceId = traceId;

        if (!string.IsNullOrWhiteSpace(type))
        {
            Type = type;
        }
    }

    public ErrorResponse(
        IEnumerable<BadRequestError> badRequestErrors,
        int? statusCode = StatusCodes.Status400BadRequest
    )
    {
        StatusCode = statusCode!.Value;
        Errors = badRequestErrors?.ToList();
        Message = "Several errors have occured";
        Type = nameof(ValidationException);
    }

    public SerializeResult Serialize() =>
        SerializerExtension.Serialize(
            this,
            options => options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        );
}
