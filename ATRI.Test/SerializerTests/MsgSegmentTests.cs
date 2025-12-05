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
        // Serialize test
        Assert.Equal(json, OneBotSerializer.Serialize(segment));
        // Deserialize test
        Assert.Equal(segment, OneBotSerializer.Deserialize<MsgSegment>(json));
    }

    [Fact]
    public void ImageTest()
    {
        // Serialize test
        var segment = new Image("https://example.com/image.png");
        var json = """
            {"type":"image","data":{"file":"https://example.com/image.png"}}
            """;
        Assert.Equal(json, OneBotSerializer.Serialize(segment));

        // Deserialize test
        var segmentRecv = new ImageRecv
        {
            File = "file_id_12345",
            Url = "https://example.com/image.png",
            FileSize = 204800,
            SubType = 0
        };
        var jsonRecv = """
            {"type":"image","data":{"file":"file_id_12345","url":"https://example.com/image.png","file_size":204800,"subType":0}}
            """;
        Assert.Equal(segmentRecv, OneBotSerializer.Deserialize<MsgSegment>(jsonRecv));
    }

    [Fact]
    public void FaceTest()
    {
        var segment = new Face(123, 1);
        var json = """
            {"type":"face","data":{"id":123,"sub_type":1}}
            """;
        // Serialize test
        Assert.Equal(json, OneBotSerializer.Serialize(segment));
        // Deserialize test
        Assert.Equal(segment, OneBotSerializer.Deserialize<MsgSegment>(json));
    }

    [Fact]
    public void AtTest()
    {
        var segment = new At(10001, "TestUser");
        var json = """
            {"type":"at","data":{"qq":"10001","name":"TestUser"}}
            """;
        // Serialize test
        Assert.Equal(json, OneBotSerializer.Serialize(segment));
        // Deserialize test
        Assert.Equal(segment, OneBotSerializer.Deserialize<MsgSegment>(json));
    }

    [Fact]
    public void AtAllTest()
    {
        var segment = new AtAll();
        var json = """
            {"type":"at","data":{"qq":"all"}}
            """;
        // Serialize test
        Assert.Equal(json, OneBotSerializer.Serialize(segment));
        // Deserialize test
        Assert.Equal(segment, OneBotSerializer.Deserialize<MsgSegment>(json));
    }
}

