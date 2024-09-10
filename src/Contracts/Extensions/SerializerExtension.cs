using System.Text.Encodings.Web;
using System.Text.Json;

namespace Contracts.Extensions;

public class SerializerExtension
{
    public static string Serialize(object data, Action<JsonSerializerOptions>? optionalOptions = null)
    {
        var options = CreateOptions();
        optionalOptions?.Invoke(options);
        return JsonSerializer.Serialize(data, options);
    }

    public static T? Deserialize<T>(string json, Action<JsonSerializerOptions>? optionalOptions = null)
    {
        var options = CreateOptions();
        optionalOptions?.Invoke(options);
        return JsonSerializer.Deserialize<T>(json, options);
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