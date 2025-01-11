using FlashDotNet.Attribute;
using FlashDotNet.Enum;
using FlashDotNet.Infrastructure;
using FlashDotNet.Jwt;
using FlashDotNet.Services.CacheService;
using FlashDotNet.SignalR.Helper;
using Microsoft.AspNetCore.SignalR;

namespace FlashDotNet.SignalR;

[AddTransient]
public class ChatHub : HubHandler
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public ChatHub(
        ILogger<ChatHub> logger,
        IJwtService jwtService,
        ICacheService cacheService
    ) : base(logger, jwtService, cacheService)
    {
    }

    /// <summary>
    /// 测试消息
    /// </summary>
    /// <param name="message"></param>
    [HubMethodName("test_message")]
    public async Task TestMessage(string message)
    {
        var connectionId = Context.ConnectionId;
        await Clients.Caller.SendAsync("test_return", $"[服务器回报] 用户 {connectionId}： {message}");
    }

    /// <summary>
    /// 广播消息
    /// </summary>
    /// <param name="message"></param>
    [AuthHub(UserRole.Admin)]
    [HubMethodName("broadcast_message")]
    public virtual async Task BroadcastMessage(string message)
    {
        await Clients.All.SendAsync("test_return", message);
    }
}
