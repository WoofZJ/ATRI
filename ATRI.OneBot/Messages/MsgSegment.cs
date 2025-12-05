using System.Text.Json.Serialization;

namespace ATRI.OneBot.Messages;

public abstract record MsgSegment;

public record PlainText(
    string Text
) : MsgSegment;

public record Image(
    string File
) : MsgSegment;

public record ImageRecv : MsgSegment
{
    public string File { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public long FileSize { get; init; }
    [JsonPropertyName("subType")]
    public int SubType { get; init; }
}