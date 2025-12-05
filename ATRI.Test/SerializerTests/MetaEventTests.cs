using ATRI.OneBot.Events;
using ATRI.OneBot.JsonConverters;

namespace ATRI.Test.SerializerTests;

public class MetaEventTests
{
    [Fact]
    public void LifeCycleEventTest()
    {
        var json = """
            {"time":1764859724,"self_id":10000,"post_type":"meta_event","meta_event_type":"lifecycle","sub_type":"connect"}
            """;
        var evt = OneBotSerializer.Deserialize<Event>(json);
        Assert.IsType<LifecycleEvent>(evt);
        Assert.Equal(evt, new LifecycleEvent
        {
            Time = 1764859724,
            SelfId = 10000,
            PostType = "meta_event",
            MetaEventType = "lifecycle",
            SubType = "connect"
        });
    }

    [Fact]
    public void HeartbeatEventTest()
    {
        var json = """
            {"time":1764860804,"self_id":10000,"post_type":"meta_event","meta_event_type":"heartbeat","status":{"online":true,"good":true},"interval":60000}
            """;
        var evt = OneBotSerializer.Deserialize<Event>(json);
        Assert.IsType<HeartbeatEvent>(evt);
        Assert.Equal(evt, new HeartbeatEvent
        {
            Time = 1764860804,
            SelfId = 10000,
            PostType = "meta_event",
            MetaEventType = "heartbeat",
            Status = new HeartbeatEvent.HeartbeatStatus
            {
                Online = true,
                Good = true
            },
            Interval = 60000
        });
    }

    [Fact]
    public void UnknownMetaEventTypeTest()
    {
        var json = """
            {"time":1764859724,"self_id":10000,"post_type":"meta_event","meta_event_type":"unknown_type"}
            """;
        Assert.Throws<NotSupportedException>(() => OneBotSerializer.Deserialize<Event>(json));
    }
}