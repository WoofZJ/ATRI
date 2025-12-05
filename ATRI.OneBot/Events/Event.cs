using ATRI.OneBot.Apis;

namespace ATRI.OneBot.Events;

public abstract record Event;

public abstract record OneBotEvent : Event
{
    public long Time { get; init; }
    public long SelfId { get; init; }
    public string PostType { get; init; } = string.Empty;
}

public interface IApiEvent<out TData> where TData : ApiData
{
    string Echo { get; }
    TData Data { get; }
}

public record ApiEvent<TData> : Event, IApiEvent<TData> where TData : ApiData
{
    public string Echo { get; init; } = string.Empty;
    public TData Data { get; init; } = default!;
}