namespace ATRI.OneBot.Messages;

public abstract record MsgSegment(string Type);

public record PlainText(string Text) : MsgSegment("text")
{
    public record TextData(
        string Text
    );
    public TextData Data => new(Text);
}

public record Face(int Id) : MsgSegment("face")
{
    public record FaceData(
        int Id
    );
    public FaceData Data => new(Id);
}
