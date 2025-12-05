using System.Text.Json;
using System.Text.Json.Serialization;
using ATRI.OneBot.Messages;

namespace ATRI.OneBot.JsonConverters;

public static class OneBotSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters =
        {
            new EventJsonConverter(),
            new MsgSegmentJsonConverter()
        }
    };

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, Options);
    }

    public static string Serialize<T>(T obj)
    {
        if (typeof(MsgSegment).IsAssignableFrom(typeof(T)))
        {
            return JsonSerializer.Serialize(obj as MsgSegment, Options);
        }
        return JsonSerializer.Serialize(obj, Options);
    }
}