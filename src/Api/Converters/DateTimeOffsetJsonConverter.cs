using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Converters;

public sealed class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    private const string IsoUtcFormat = "yyyy-MM-dd'T'HH':'mm':'ss.fff'Z'";

    public override DateTimeOffset Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        try
        {
            return reader.GetDateTimeOffset();
        }
        catch (Exception ex)
        {
            throw new JsonException(
                $"Invalid DateTimeOffset value. Expected ISO-8601 UTC format ({IsoUtcFormat}).",
                ex
            );
        }
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTimeOffset value,
        JsonSerializerOptions options
    ) => writer.WriteStringValue(value.ToUniversalTime().ToString(IsoUtcFormat));
}
