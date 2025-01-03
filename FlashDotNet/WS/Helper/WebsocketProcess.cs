using FlashDotNet.DTOs.WebSocket;
using FlashDotNet.Enum;
using FlashDotNet.Infrastructure;

namespace FlashDotNet.WS.Helper;

/// <summary>
/// websocket请求处理
/// </summary>
[AddScoped]
public class WebsocketProcess
{
    private readonly WsRouteResolver _routeResolver;
    private readonly ILogger<WebsocketProcess> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public WebsocketProcess(WsRouteResolver routeResolver, ILogger<WebsocketProcess> logger)
    {
        _routeResolver = routeResolver;
        _logger = logger;
    }

    /// <summary>
    /// 调用此方法去处理某个 WebSocket 请求
    /// </summary>
    public async Task Process(UserConnection socket, WsReq data)
    {
        try
        {
            WsRoute? route = data.Route;
            _logger.LogInformation($"收到路由: {route}, 原字符串: {data.Route}");

            // 找到对应的处理器
            var handler = _routeResolver.GetHandler(route);
            if (handler == null)
            {
                // 无对应处理器
                var errorMsg = new WsError<object>
                {
                    Message = "请求路由错误，请检查路由是否正确",
                    Data = null
                };
                await socket.SendMessageAsync(errorMsg);

                // 看需求是否断开
                // await socket.DisconnectAsync();
                return;
            }

            // 调用处理器
            await handler.HandleAsync(socket, data);
        }
        catch (Exception ex)
        {
            // 转换枚举时或处理时可能抛异常
            _logger.LogError(ex, "WebSocket消息处理异常");

            var errorMsg = new WsError<object>
            {
                Message = "服务器处理请求时发生异常",
                Data = new
                {
                    exception = ex.Message
                }
            };
            await socket.SendMessageAsync(errorMsg);
        }
    }
}
