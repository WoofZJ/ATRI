using Fleck;
using ATRI.OneBot.Events;
using ATRI.OneBot.JsonConverters;
using ATRI.OneBot.Apis;
using ATRI.OneBot.Messages;

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

    private  IWebSocketConnection? Socket { get; set; }

    private readonly Dictionary<string, TaskCompletionSource<ApiData>> PendingApiCalls = [];

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
                Socket = socket;
                Subscribe(
                    [typeof(IApiEvent<ApiData>)],
                    100,
                    async (evt, adapter) =>
                    {
                        var apiEvent = evt as IApiEvent<ApiData>;
                        if (apiEvent == null || !PendingApiCalls.TryGetValue(apiEvent.Echo, out TaskCompletionSource<ApiData>? tcs))
                        {
                            return false;
                        }
                        tcs.SetResult(apiEvent.Data);
                        PendingApiCalls.Remove(apiEvent.Echo);
                        return false;
                    }
                );
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

    public async Task<TData> CallApiAsync<TPayload, TData>(string action, TPayload payload) where TPayload : ApiPayload where TData : ApiData
    {
        if (Socket == null) throw new InvalidOperationException("Socket is not connected.");
        var echo = $"{action}/{Guid.NewGuid():N}";
        PendingApiCalls[echo] = new TaskCompletionSource<ApiData>();
        Socket?.Send(OneBotSerializer.Serialize(new
        {
            action,
            @params = payload,
            echo
        }));
        return (TData)await PendingApiCalls[echo].Task;
    }

    public async Task<SendGroupMsgData> SendGroupMsgAsync(long groupId, MsgChain message)
        => await CallApiAsync<SendGroupMsgPayload, SendGroupMsgData>("send_group_msg", new(groupId, message));

    public async Task<SendPrivateMsgData> SendPrivateMsgAsync(long userId, MsgChain message)
        => await CallApiAsync<SendPrivateMsgPayload, SendPrivateMsgData>("send_private_msg", new(userId, message));

    public async Task<SendMsgData> SendMsgAsync(
        string messageType, long? userId = null, long? groupId = null, MsgChain? message = null)
        => await CallApiAsync<SendMsgPayload, SendMsgData>("send_msg", new(messageType, userId, groupId, message));

    public async Task<DeleteMsgData> DeleteMsgAsync(long messageId)
        => await CallApiAsync<DeleteMsgPayload, DeleteMsgData>("delete_msg", new(messageId));

    public async Task<GetMsgData> GetMsgAsync(long messageId)
        => await CallApiAsync<GetMsgPayload, GetMsgData>("get_msg", new(messageId));
    
    public async Task<SendLikeData> SendLikeAsync(long userId, int times = 1)
        => await CallApiAsync<SendLikePayload, SendLikeData>("send_like", new(userId, times));

    public async Task<SetGroupKickData> SetGroupKickAsync(long groupId, long userId, bool rejectAddRequest = false)
        => await CallApiAsync<SetGroupKickPayload, SetGroupKickData>("set_group_kick", new(groupId, userId, rejectAddRequest));

    public async Task<SetGroupBanData> SetGroupBanAsync(long groupId, long userId, int duration = 30 * 60)
        => await CallApiAsync<SetGroupBanPayload, SetGroupBanData>("set_group_ban", new(groupId, userId, duration));

    public async Task<SetGroupWholeBanData> SetGroupWholeBanAsync(long groupId, bool enable = true)
        => await CallApiAsync<SetGroupWholeBanPayload, SetGroupWholeBanData>("set_group_whole_ban", new(groupId, enable));
    
    public async Task<SetGroupAdminData> SetGroupAdminAsync(long groupId, long userId, bool enable = true)
        => await CallApiAsync<SetGroupAdminPayload, SetGroupAdminData>("set_group_admin", new(groupId, userId, enable));
    
    public async Task<SetGroupCardData> SetGroupCardAsync(long groupId, long userId, string card)
        => await CallApiAsync<SetGroupCardPayload, SetGroupCardData>("set_group_card", new(groupId, userId, card));

    public async Task<SetGroupNameData> SetGroupNameAsync(long groupId, string groupName)
        => await CallApiAsync<SetGroupNamePayload, SetGroupNameData>("set_group_name", new(groupId, groupName));
    
    public async Task<SetGroupLeaveData> SetGroupLeaveAsync(long groupId, bool isDismiss = false)
        => await CallApiAsync<SetGroupLeavePayload, SetGroupLeaveData>("set_group_leave", new(groupId, isDismiss));
    
    public async Task<SetGroupSpecialTitleData> SetGroupSpecialTitleAsync(long groupId, long userId, string specialTitle, int duration = -1)
        => await CallApiAsync<SetGroupSpecialTitlePayload, SetGroupSpecialTitleData>("set_group_special_title", new(groupId, userId, specialTitle, duration));
    
    public async Task<SetFriendAddRequestData> SetFriendAddRequestAsync(string flag, bool approve = true, string? remark = null)
        => await CallApiAsync<SetFriendAddRequestPayload, SetFriendAddRequestData>("set_friend_add_request", new(flag, approve, remark));
    
    public async Task<SetGroupAddRequestData> SetGroupAddRequestAsync(string flag, string subType, bool approve = true, string? reason = null)
        => await CallApiAsync<SetGroupAddRequestPayload, SetGroupAddRequestData>("set_group_add_request", new(flag, subType, approve, reason));
    
    public async Task<GetLoginInfoData> GetLoginInfoAsync()
        => await CallApiAsync<GetLoginInfoPayload, GetLoginInfoData>("get_login_info", new());
    
    public async Task<GetStrangerInfoData> GetStrangerInfoAsync(long userId, bool noCache = false)
        => await CallApiAsync<GetStrangerInfoPayload, GetStrangerInfoData>("get_stranger_info", new(userId, noCache));
    

    public async Task<GetFriendListData> GetFriendListAsync()
        => await CallApiAsync<GetFriendListPayload, GetFriendListData>("get_friend_list", new());
    
    public async Task<GetGroupInfoData> GetGroupInfoAsync(long groupId, bool noCache = false)
        => await CallApiAsync<GetGroupInfoPayload, GetGroupInfoData>("get_group_info", new(groupId, noCache));
    
    public async Task<GetGroupListData> GetGroupListAsync()
        => await CallApiAsync<GetGroupListPayload, GetGroupListData>("get_group_list", new());
    
    public async Task<GetGroupMemberInfoData> GetGroupMemberInfoAsync(long groupId, long userId, bool noCache = false)
        => await CallApiAsync<GetGroupMemberInfoPayload, GetGroupMemberInfoData>("get_group_member_info", new(groupId, userId, noCache));
    
    public async Task<GetGroupMemberListData> GetGroupMemberListAsync(long groupId)
        => await CallApiAsync<GetGroupMemberListPayload, GetGroupMemberListData>("get_group_member_list", new(groupId));

}