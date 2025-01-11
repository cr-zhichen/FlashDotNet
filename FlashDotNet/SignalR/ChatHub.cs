using FlashDotNet.Attribute;
using FlashDotNet.Enum;
using Microsoft.AspNetCore.SignalR;

namespace FlashDotNet.SignalR;

public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        Console.WriteLine($"[SignalR] 用户 {connectionId} 连接成功");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        Console.WriteLine($"[SignalR] 用户 {connectionId} 断开连接");
        await base.OnDisconnectedAsync(exception);
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
    [Auth(UserRole.Admin)]
    [HubMethodName("broadcast_message")]
    public async Task BroadcastMessage(string message)
    {
        await Clients.All.SendAsync("test_return", message);
    }
}
