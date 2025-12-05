using ATRI.OneBot.Messages;
using ATRI.OneBot.JsonConverters;

namespace ATRI.Test.SerializerTests;

public class MsgSegmentTests
{
    [Fact]
    public void PlainTextTest()
    {
        var segment = new PlainText("Hello, World!");
        var json = """
            {"type":"text","data":{"text":"Hello, World!"}}
            """;
        // Deserialize test
        Assert.Equal(json, OneBotSerializer.Serialize(segment));
        // Serialize test
        Assert.Equal(segment, OneBotSerializer.Deserialize<MsgSegment>(json));
    }
}

