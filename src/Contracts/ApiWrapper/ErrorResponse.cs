using System.Text.Json;
using System.Text.Json.Serialization;
using Contracts.Extensions;
using Microsoft.AspNetCore.Http;
namespace Contracts.ApiWrapper;

public class ErrorResponse : ApiBaseResponse
{
    public string Type { get; } = "InternalServerException";

    public string? TraceId { get; set; }

    public object? Exception { get; set; }

    public ICollection<BadRequestError>? Errors { get; }

    public ErrorResponse(
        string message,
        string? type = null,
        string? traceId = null,
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
        string? type = null,
        string? message = null!,
        string? traceId = null!,
        int? statusCode = StatusCodes.Status400BadRequest
    )
    {
        StatusCode = statusCode!.Value;
        Errors = badRequestErrors?.ToList();
        Message = message ?? "One or several errors have occured";
        Type = type ?? "BadRequestException";
        TraceId = traceId;
    }

    public override string ToString() => SerializerExtension.Serialize(
            this,
            ActionOptions
        ).StringJson;

    public JsonSerializerOptions GetOptions() => SerializerExtension.Options(ActionOptions);
    

    private readonly Action<JsonSerializerOptions> ActionOptions = options =>
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
}
