using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace ATRI.OneBot.Events;

[JsonConverter(typeof(EventJsonConverter))]
public abstract record Event
{
    public long Time { get; init; }
    public long SelfId { get; init; }
    public string PostType { get; init; } = string.Empty;
}

public class EventJsonConverter : JsonConverter<Event>
{
    public override Event? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonNode.Parse(ref reader);
        return node?["post_type"]?.GetValue<string>() switch
        {
            "meta_event" => node?["meta_event_type"]?.GetValue<string>() switch
            {
                "heartbeat" => node.Deserialize<HeartbeatEvent>(options),
                "lifecycle" => node.Deserialize<LifecycleEvent>(options),
                _ => throw new NotSupportedException("Unsupported meta_event_type")
            },
            _ => null
        };
    }

    public override void Write(Utf8JsonWriter writer, Event value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }
}