using System.Text.Encodings.Web;
using System.Text.Json;

namespace Contracts.Extensions;

public class SerializerExtension
{
    public static string Serialize(object data)
    {
        return JsonSerializer.Serialize(data, CreateOptions());
    }

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, CreateOptions());
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