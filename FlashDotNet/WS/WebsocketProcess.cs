using FlashDotNet.DTOs;
using FlashDotNet.DTOs.WebSocket.Requests;
using FlashDotNet.Utils;
using Newtonsoft.Json.Linq;
using Route = FlashDotNet.Enum.Route;

namespace FlashDotNet.WS;

/// <summary>
/// websocket请求处理
/// </summary>
public static class WebsocketProcess
{
    private readonly static Dictionary<string, Func<UserConnection, WsReq, Task>> RouteHandlers = new Dictionary<string, Func<UserConnection, WsReq, Task>>
    {
        { Route.Test.GetDisplayName(), HandleTestRoute }
    };

    /// <summary>
    /// 处理WebSocket请求
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="data"></param>
    public static async Task Process(UserConnection socket, WsReq data)
    {
        if (RouteHandlers.TryGetValue(data.Route ?? "", out var handler))
        {
            await handler(socket, data);
        }
        else
        {
            await socket.SendMessageAsync(JObject.FromObject(new WsError<JObject>
            {
                Route = Route.Error.GetDisplayName(),
                Message = "请求路由错误，请检查路由是否正确",
            }));
            
            await socket.DisconnectAsync();
        }
    }

    private static async Task HandleTestRoute(UserConnection socket, WsReq data)
    {
        await socket.SendMessageAsync(JObject.FromObject(new WsOk<JObject>
        {
            Route = Route.Test.GetDisplayName(),
            Data = data.Data,
        }));
    }
}
