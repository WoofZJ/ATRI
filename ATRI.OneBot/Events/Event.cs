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
            "notice" => node?["notice_type"]?.GetValue<string>() switch
            {
                "group_upload" => node.Deserialize<GroupUploadEvent>(options),
                "group_admin" => node.Deserialize<GroupAdminEvent>(options),
                "group_decrease" => node.Deserialize<GroupDecreaseEvent>(options),
                "group_increase" => node.Deserialize<GroupIncreaseEvent>(options),
                "group_ban" => node.Deserialize<GroupBanEvent>(options),
                "friend_add" => node.Deserialize<FriendAddEvent>(options),
                "group_recall" => node.Deserialize<GroupRecallEvent>(options),
                "friend_recall" => node.Deserialize<FriendRecallEvent>(options),
                "notify" => node?["sub_type"]?.GetValue<string>() switch
                {
                    "poke" => node.Deserialize<PokeEvent>(options),
                    "lucky_king" => node.Deserialize<LuckyKingEvent>(options),
                    "honor" => node.Deserialize<HonorEvent>(options),
                    _ => throw new NotSupportedException("Unsupported notify sub_type")
                },
                _ => throw new NotSupportedException("Unsupported notice_type")
            },
            "message" => node?["message_type"]?.GetValue<string>() switch
            {
                "private" => node.Deserialize<PrivateMessageEvent>(options),
                "group" => node.Deserialize<GroupMessageEvent>(options),
                _ => throw new NotSupportedException("Unsupported message_type")
            },
            _ => throw new NotSupportedException("Unsupported post_type")
        };
    }

    public override void Write(Utf8JsonWriter writer, Event value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }
}