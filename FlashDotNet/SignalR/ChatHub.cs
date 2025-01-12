using FlashDotNet.Attribute;
using FlashDotNet.Enum;
using FlashDotNet.Infrastructure;
using FlashDotNet.Jwt;
using FlashDotNet.Services.CacheService;
using FlashDotNet.SignalR.Helper;
using FlashDotNet.Utils;
using Microsoft.AspNetCore.SignalR;

namespace FlashDotNet.SignalR;

/// <summary>
/// 聊天室 Hub
/// </summary>
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
    [SignalRHubName(SignalRRoute.Test)]
    public async Task Test(string message)
    {
        var connectionId = Context.ConnectionId;
        await SendAsync(SignalRRoute.Test, $"[服务器回报] 用户 {connectionId}： {message}");
    }

    /// <summary>
    /// 广播消息
    /// </summary>
    /// <param name="message"></param>
    [AuthHub(UserRole.Admin)]
    [SignalRHubName(SignalRRoute.BroadcastMessage)]
    public virtual async Task Broadcast(string message)
    {
        await SendAllAsync(SignalRRoute.Test, message);
    }
}
