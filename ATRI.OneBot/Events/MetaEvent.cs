namespace ATRI.OneBot.Events;

public abstract record MetaEvent : Event
{
    public string MetaEventType { get; init; } = string.Empty;
}

public record HeartbeatEvent : MetaEvent
{
    public int Interval { get; init; }
    public record HeartbeatStatus
    {
        public bool Online { get; init; }
        public bool Good { get; init; }
    }
    public HeartbeatStatus Status { get; init; } = new HeartbeatStatus();
}

public record LifecycleEvent : MetaEvent
{
    public string SubType { get; init; } = string.Empty;
}