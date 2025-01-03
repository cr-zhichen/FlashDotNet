using FlashDotNet.Attribute;
using FlashDotNet.DTOs;
using FlashDotNet.DTOs.WebSocket.Requests;
using FlashDotNet.Enum;
using FlashDotNet.Infrastructure;
using FlashDotNet.Utils;
using Newtonsoft.Json.Linq;

namespace FlashDotNet.WS.RouteDispose;

/// <summary>
/// 测试路由处理器
/// </summary>
[AddScopedAsImplementedInterfaces]
public class TestRouteHandler : IWsRouteHandler
{
    private readonly ILogger<TestRouteHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public TestRouteHandler(ILogger<TestRouteHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 声明路由
    /// </summary>
    public WsRoute Route => WsRoute.Test;

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="userConnection"></param>
    /// <param name="data"></param>
    public async Task HandleAsync(UserConnection userConnection, WsReq data)
    {
        _logger.LogInformation("[TestRoute] 开始处理请求...");

        var response = new WsOk<JObject>
        {
            Route = WsRoute.Test.GetDisplayName(),
            Data = data.Data
        };

        await userConnection.SendMessageAsync(JObject.FromObject(response));
    }
}
