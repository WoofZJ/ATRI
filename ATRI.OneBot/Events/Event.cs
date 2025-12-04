using System.Text.Json.Serialization;

namespace ATRI.OneBot.Events;

[JsonConverter(typeof(EventJsonConverter))]
public abstract record Event
{
    public long Time { get; init; }
    public long SelfId { get; init; }
    public string PostType { get; init; } = string.Empty;
}
