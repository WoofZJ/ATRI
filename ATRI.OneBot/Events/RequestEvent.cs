using System.ComponentModel.DataAnnotations;

namespace ATRI.OneBot.Events;

public abstract record RequestEvent : OneBotEvent
{
    public string RequestType { get; init; } = string.Empty;
}

public record FriendRequestEvent : RequestEvent
{
    public long UserId { get; init; }
    public string Comment { get; init; } = string.Empty;
    public string Flag { get; init; } = string.Empty;
}

public record GroupRequestEvent : RequestEvent
{
    [AllowedValues("add", "invite")]
    public string SubType { get; init; } = string.Empty;
    public long GroupId { get; init; }
    public long UserId { get; init; }
    public string Comment { get; init; } = string.Empty;
    public string Flag { get; init; } = string.Empty;
}