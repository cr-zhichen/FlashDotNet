using System.Net.WebSockets;
using FlashDotNet.DTOs.WebSocket;

namespace FlashDotNet.WS.Helper;

/// <summary>
/// 用户连接
/// </summary>
public class UserConnection
{
    /// <summary>
    /// WebSocket
    /// </summary>
    public required WebSocket WebSocket { get; init; }

    /// <summary>
    /// SocketId
    /// </summary>
    public required string SocketId { get; init; }

    /// <summary>
    /// 鉴权Token，可空
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="data"></param>
    public async Task SendMessageAsync<T>(IWsRe<T> data)
    {
        await WebSocketController.SendAsync(WebSocket, data);
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public async Task DisconnectAsync()
    {
        if (WebSocket.State != WebSocketState.Open)
            throw new Exception("WebSocket is not open.");

        await WebSocketController.CloseWebSocketAsync(SocketId);
    }
}
