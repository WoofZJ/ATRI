using Fleck;
using ATRI.OneBot.Events;
using ATRI.OneBot.JsonConverters;

namespace ATRI.OneBot;

public class OneBotAdapter
{
    private class EventDispatcher
    {
        public HashSet<Type> EventTypes { get; set; } = [];
        public Func<Event, OneBotAdapter, Task<bool>> Handler { get; set; } = default!;
        public int Priority { get; set; }
    }

    private readonly List<EventDispatcher> EventDispatchers = [];

    public OneBotAdapter(string url, int port, string suffix, string token)
    {
        var server = new WebSocketServer($"ws://{url}:{port}");
        server.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                string path = socket.ConnectionInfo.Path;
                string? query = socket.ConnectionInfo.Headers["Authorization"].Split(' ').LastOrDefault();
                if (path != suffix || query != token)
                {
                    socket.Close();
                    Console.WriteLine("Connection rejected due to invalid path or token.");
                    return;
                }
                Console.WriteLine("Connection established!");
            };
            socket.OnClose = () => Console.WriteLine("Connection closed!");
            socket.OnMessage = async message =>
            {
                Console.WriteLine($"Received message: {message}");
                var evt = OneBotSerializer.Deserialize<Event>(message);
                if (evt == null) return;
                Type type = evt.GetType();
                foreach (var dispatcher in EventDispatchers)
                {
                    if (dispatcher.EventTypes.Any(t => t.IsAssignableFrom(type)))
                    {
                        if (await dispatcher.Handler(evt, this))
                        {
                            break;
                        }
                    }
                }
            };
        });
    }

    public void Subscribe(HashSet<Type> eventTypes, int priority, Func<Event, OneBotAdapter, Task<bool>> handler)
    {
        EventDispatchers.Add(new EventDispatcher
        {
            EventTypes = eventTypes,
            Handler = handler,
            Priority = priority
        });
        EventDispatchers.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }
}