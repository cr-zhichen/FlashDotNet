namespace FlashDotNet.Attribute;

/// <summary>
/// 自定义 WebSocket 路由特性，用于标记方法映射到哪个路由
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class WsRouteAttribute : System.Attribute
{
    /// <summary>
    /// WebSocket 路由
    /// </summary>
    public string Route { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="route"></param>
    public WsRouteAttribute(string route)
    {
        Route = route;
    }
}
