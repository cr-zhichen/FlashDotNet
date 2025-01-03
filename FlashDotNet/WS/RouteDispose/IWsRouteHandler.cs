using System.Net.WebSockets;
using FlashDotNet.DTOs.WebSocket;
using FlashDotNet.Enum;

namespace FlashDotNet.WS.RouteDispose;

/// <summary>
/// 定义WebSocket路由处理接口
/// </summary>
public interface IWsRouteHandler
{
    /// <summary>
    /// 此处理器负责处理的路由枚举值
    /// </summary>
    WsRoute Route { get; }

    /// <summary>
    /// 处理请求的核心逻辑
    /// </summary>
    /// <param name="userConnection">用户连接包装对象</param>
    /// <param name="data">请求数据</param>
    Task HandleAsync(UserConnection userConnection, WsReq data);
}
