using System.ComponentModel.DataAnnotations;

namespace ATRI.OneBot.Events;

public abstract record NoticeEvent : OneBotEvent
{
    public string NoticeType { get; init; } = string.Empty;
}

public record GroupUploadEvent : NoticeEvent
{
    public long GroupId { get; init; }
    public long UserId { get; init; }
    public record FileInfo
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public long Size { get; init; }
        public long BusId { get; init; }
    }
    public FileInfo File { get; init; } = new FileInfo();
}

public record GroupAdminEvent : NoticeEvent
{
    [AllowedValues("set", "unset")]
    public string SubType { get; init; } = string.Empty;
    public long GroupId { get; init; }
    public long UserId { get; init; }
}

public record GroupDecreaseEvent : NoticeEvent
{
    [AllowedValues("leave", "kick", "kick_me")]
    public string SubType { get; init; } = string.Empty;
    public long GroupId { get; init; }
    public long OperatorId { get; init; }
    public long UserId { get; init; }
}

public record GroupIncreaseEvent : NoticeEvent
{
    [AllowedValues("approve", "invite")]
    public string SubType { get; init; } = string.Empty;
    public long GroupId { get; init; }
    public long OperatorId { get; init; }
    public long UserId { get; init; }
}

public record GroupBanEvent : NoticeEvent
{
    [AllowedValues("ban", "lift_ban")]
    public string SubType { get; init; } = string.Empty;
    public long GroupId { get; init; }
    public long OperatorId { get; init; }
    public long UserId { get; init; }
    public long Duration { get; init; }
}

public record FriendAddEvent : NoticeEvent
{
    public long UserId { get; init; }
}

public record GroupRecallEvent : NoticeEvent
{
    public long GroupId { get; init; }
    public long UserId { get; init; }
    public long OperatorId { get; init; }
    public long MessageId { get; init; }
}

public record FriendRecallEvent : NoticeEvent
{
    public long UserId { get; init; }
    public long MessageId { get; init; }
}

public record NotifyEvent : NoticeEvent
{
    public string SubType { get; init; } = string.Empty;
}

public record PokeEvent : NotifyEvent
{
    public long GroupId { get; init; }
    public long UserId { get; init; }
    public long TargetId { get; init; }
}

public record LuckyKingEvent : NotifyEvent
{
    public long GroupId { get; init; }
    public long UserId { get; init; }
    public long TargetId { get; init; }
}

public record HonorEvent : NotifyEvent
{
    public long GroupId { get; init; }
    public long UserId { get; init; }
    [AllowedValues("talkative", "performer", "emotion")]
    public string HonorType { get; init; } = string.Empty;
}