using ATRI.OneBot.Events;
using ATRI.OneBot.JsonConverters;

namespace ATRI.Test.SerializerTests;

public class MessageEventTests
{
    [Fact]
    public void GroupMessageEventTest()
    {
        var json = """
            {"self_id":10000,"user_id":10000,"time":1764861194,"message_id":-100000,"message_seq":98099,"message_type":"group","sender":{"user_id":10001,"nickname":"ForTest","card":"","role":"member","title":""},"raw_message":"1","font":14,"sub_type":"normal","message":[{"type":"text","data":{"text":"1"}}],"message_format":"array","post_type":"message","raw_pb":"","group_id":10000,"group_name":"Robots"}
            """;
        var evt = OneBotSerializer.Deserialize<Event>(json);
        Assert.Equal(typeof(GroupMessageEvent), evt?.GetType());
        var ground = new GroupMessageEvent
        {
            SelfId = 10000,
            UserId = 10000,
            Time = 1764861194,
            PostType = "message",
            MessageId = -100000,
            MessageSeq = 98099,
            MessageType = "group",
            SubType = "normal",
            Message = new("1"),
            RawMessage = "1",
            Font = 14,
            GroupId = 10000,
            GroupName = "Robots",
            Sender = new GroupMessageEvent.SenderInfo
            {
                UserId = 10001,
                Nickname = "ForTest",
                Card = string.Empty,
                Role = "member",
                Title = string.Empty
            }
        };
        Assert.Equal(ground.Message, (evt as GroupMessageEvent)?.Message);
    }

    [Fact]
    public void PrivateMessageEventTest()
    {
        var json = """
            {"self_id":10000,"user_id":10001,"time":1764903293,"message_id":-1234234,"message_seq":100,"message_type":"private","sender":{"user_id":10001,"nickname":"ForTest","card":""},"raw_message":"1","font":14,"sub_type":"friend","message":[{"type":"text","data":{"text":"1"}}],"message_format":"array","post_type":"message","raw_pb":""}
            """;
        var evt = OneBotSerializer.Deserialize<Event>(json);
        Assert.Equal(typeof(PrivateMessageEvent), evt?.GetType());
        var ground = new PrivateMessageEvent
        {
            SelfId = 10000,
            UserId = 10001,
            Time = 1764903293,
            PostType = "message",
            MessageId = -1234234,
            MessageSeq = 100,
            MessageType = "private",
            SubType = "friend",
            Message = new("1"),
            RawMessage = "1",
            Font = 14,
            Sender = new PrivateMessageEvent.SenderInfo
            {
                UserId = 10001,
                Nickname = "ForTest",
                Card = string.Empty
            }
        };
        Assert.Equal(ground.Message, (evt as PrivateMessageEvent)?.Message);
    }
}