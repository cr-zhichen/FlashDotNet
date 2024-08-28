using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json.Linq;

namespace FlashDotNet.WS;

/// <summary>
/// 用户连接
/// </summary>
public class UserConnection
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="webSocket"></param>
    /// <param name="socketId"></param>
    public UserConnection(WebSocket webSocket, string socketId)
    {
        WebSocket = webSocket;
        SocketId = socketId;
    }

    /// <summary>
    /// WebSocket
    /// </summary>
    public WebSocket WebSocket { get; }

    /// <summary>
    /// 连接ID
    /// </summary>
    public string SocketId { get; }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="message"></param>
    public async Task SendMessageAsync(JObject message)
    {
        if (WebSocket.State != WebSocketState.Open)
            return;

        var buffer = Encoding.UTF8.GetBytes(message.ToString());
        await WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true,
            CancellationToken.None);
    }
}
