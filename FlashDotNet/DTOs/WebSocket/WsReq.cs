using FlashDotNet.Enum;

namespace FlashDotNet.DTOs.WebSocket;

/// <summary>
/// websocket请求
/// </summary>
public class WsReq
{
    /// <summary>
    /// websocket请求的路由
    /// </summary>
    public WsRoute? Route { get; set; }

    /// <summary>
    /// 请求数据
    /// </summary>
    public object? Data { get; set; }
}
