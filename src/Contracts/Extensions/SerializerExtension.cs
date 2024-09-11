using System.Text.Encodings.Web;
using System.Text.Json;

namespace Contracts.Extensions;

public class SerializerExtension
{
    public static SerializeResult Serialize(
        object data,
        Action<JsonSerializerOptions>? optionalOptions = null
    )
    {
        var options = CreateOptions();
        optionalOptions?.Invoke(options);
        return new(JsonSerializer.Serialize(data, options), options);
    }

    public static DeserializeResult<T?> Deserialize<T>(
        string json,
        Action<JsonSerializerOptions>? optionalOptions = null
    )
    {
        var options = CreateOptions();
        optionalOptions?.Invoke(options);
        return new(JsonSerializer.Deserialize<T>(json, options), options);
    }

    private static JsonSerializerOptions CreateOptions() =>
        new()
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
}

public record SerializeResult(string StringJson, JsonSerializerOptions Options);

public record DeserializeResult<T>(T? Object, JsonSerializerOptions Options);
