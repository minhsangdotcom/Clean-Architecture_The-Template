using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Converters;

public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    private const string IsoUtcFormat = "yyyy-MM-dd'T'HH':'mm':'ss'Z'";

    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var str = reader.GetString()!;
        try
        {
            return DateTime.ParseExact(
                str,
                IsoUtcFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
            );
        }
        catch (FormatException ex)
        {
            throw new JsonException($"Invalid DateTime value. Expected format {IsoUtcFormat}.", ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var utc = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        writer.WriteStringValue(utc.ToString(IsoUtcFormat, CultureInfo.InvariantCulture));
    }
}
