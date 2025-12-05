namespace ATRI.OneBot.Events;

public abstract record Event;

public abstract record OneBotEvent : Event
{
    public long Time { get; init; }
    public long SelfId { get; init; }
    public string PostType { get; init; } = string.Empty;
}
