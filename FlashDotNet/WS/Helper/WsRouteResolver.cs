using FlashDotNet.Enum;
using FlashDotNet.Infrastructure;
using FlashDotNet.WS.RouteDispose;

namespace FlashDotNet.WS.Helper;

/// <summary>
/// WebSocket 路由解析器
/// </summary>
[AddScoped]
public class WsRouteResolver
{
    private readonly Dictionary<WsRoute, IWsRouteHandler> _handlerMap;

    /// <summary>
    /// 构造函数中，通过 DI 拿到所有已注册的 IWsRouteHandler
    /// 并根据它们声明的 WsRoute 枚举值做映射
    /// </summary>
    public WsRouteResolver(IEnumerable<IWsRouteHandler> handlers)
    {
        _handlerMap = handlers.ToDictionary(h => h.Route, h => h);
    }

    /// <summary>
    /// 根据给定的枚举路由获取对应的处理器
    /// </summary>
    /// <param name="route">WsRoute枚举值</param>
    /// <returns>若找到处理器返回它，否则返回 null</returns>
    public IWsRouteHandler? GetHandler(WsRoute? route)
    {
        if (route is null)
        {
            return null;
        }

        return _handlerMap.TryGetValue(route.Value, out var handler) ? handler : null;
    }
}
