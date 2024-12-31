using System.Text.Json;
using System.Text.Json.Serialization;
using Contracts.Extensions;
using Microsoft.AspNetCore.Http;

namespace Contracts.ApiWrapper;

public class ErrorResponse : ApiBaseResponse
{
    public string Type { get; } = "InternalServerException";

    public TraceLogging? Trace { get; set; }

    public object? Exception { get; set; }

    public ICollection<BadRequestError>? Errors { get; init; }

    public ErrorResponse() { }

    public ErrorResponse(
        string message,
        string? type = null,
        TraceLogging? trace = null,
        object? exception = null,
        int? statusCode = StatusCodes.Status500InternalServerError
    )
    {
        StatusCode = statusCode!.Value;
        Exception = exception;
        Message = message;
        Trace = trace;

        if (!string.IsNullOrWhiteSpace(type))
        {
            Type = type;
        }
    }

    public ErrorResponse(
        IEnumerable<BadRequestError> badRequestErrors,
        string? type = null,
        string? message = null!,
        TraceLogging? trace = null!,
        int? statusCode = StatusCodes.Status400BadRequest
    )
    {
        StatusCode = statusCode!.Value;
        Errors = badRequestErrors?.ToList();
        Message = message ?? "One or several errors have occured";
        Type = type ?? "BadRequestException";
        Trace = trace;
    }

    public override string ToString() =>
        SerializerExtension.Serialize(this, ActionOptions).StringJson;

    public JsonSerializerOptions GetOptions() => SerializerExtension.Options(ActionOptions);

    private readonly Action<JsonSerializerOptions> ActionOptions = options =>
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
}
