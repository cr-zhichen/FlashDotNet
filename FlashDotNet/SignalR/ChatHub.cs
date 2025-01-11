using FlashDotNet.Attribute;
using FlashDotNet.Enum;
using Microsoft.AspNetCore.SignalR;

namespace FlashDotNet.SignalR;

public class ChatHub : Hub
{
    /// <summary>
    /// 测试消息
    /// </summary>
    /// <param name="message"></param>
    [HubMethodName("test_message")]
    public async Task TestMessage(string message)
    {
        await Clients.Caller.SendAsync("test_message", $"[服务器回报] {message}");
        await Clients.All.SendAsync("test_message", $"[服务器广播] {message}");
    }
}
