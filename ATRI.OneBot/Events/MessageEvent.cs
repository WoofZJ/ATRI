using System.ComponentModel.DataAnnotations;

namespace ATRI.OneBot.Events;

public abstract record MessageEvent : Event
{
    public string MessageType { get; init; } = string.Empty;
}

public record PrivateMessageEvent : MessageEvent
{
    [AllowedValues("friend", "group", "other")]
    public string SubType { get; init; } = string.Empty;
    public long MessageId { get; init; }
    public long UserId { get; init; }
    public object Message { get; init; } = new();
    public string RawMessage { get; init; } = string.Empty;
    public int Font { get; init; }
    public record SenderInfo
    {
        public long UserId { get; init; }
        public string Nickname { get; init; } = string.Empty;
        public string Sex { get; init; } = string.Empty;
        public int Age { get; init; }
    }
    public SenderInfo Sender { get; init; } = new SenderInfo();
}

public record GroupMessageEvent : MessageEvent
{
    [AllowedValues("normal", "anonymous", "notice")]
    public string SubType { get; init; } = string.Empty;
    public long MessageId { get; init; }
    public long GroupId { get; init; }
    public long UserId { get; init; }
    public record AnonymousInfo
    {
        public long Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Flag { get; init; } = string.Empty;
    }
    public AnonymousInfo? Anonymous { get; init; }
    public object Message { get; init; } = new();
    public string RawMessage { get; init; } = string.Empty;
    public int Font { get; init; }
    public record SenderInfo
    {
        public long UserId { get; init; }
        public string Nickname { get; init; } = string.Empty;
        public string Card { get; init; } = string.Empty;
        public string Sex { get; init; } = string.Empty;
        public int Age { get; init; }
        public string Area { get; init; } = string.Empty;
        public string Level { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
    }
    public SenderInfo Sender { get; init; } = new SenderInfo();
}