namespace ATRI.OneBot.Messages;

public abstract record MsgSegment;

public record PlainText(
    string Text
) : MsgSegment;
