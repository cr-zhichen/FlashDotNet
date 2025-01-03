using FlashDotNet.DTOs.WebSocket;
using FlashDotNet.Enum;
using FlashDotNet.Infrastructure;
using FlashDotNet.Jwt;

namespace FlashDotNet.WS.Helper;

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
    /// 初始化
    /// </summary>
    /// <param name="userConnection">用户连接包装对象</param>
    /// <param name="data">请求数据</param>
    /// <returns></returns>
    Task InitializeAsync(UserConnection userConnection, WsReq data);

    /// <summary>
    /// 处理请求的核心逻辑
    /// </summary>
    /// <param name="userConnection">用户连接包装对象</param>
    /// <param name="data">请求数据</param>
    Task HandleAsync(UserConnection userConnection, WsReq data);
}

/// <inheritdoc />
[AddScopedAsImplementedInterfaces]
public abstract class WsRouteHandler : IWsRouteHandler
{
    /// <summary>
    /// JWT服务
    /// </summary>
    protected readonly IJwtService JwtService;

    /// <summary>
    /// 构造函数
    /// </summary>
    protected WsRouteHandler(IJwtService jwtService)
    {
        JwtService = jwtService;
    }

    /// <inheritdoc />
    public abstract WsRoute Route { get; }

    /// <summary>
    /// 此处理器负责处理的用户角色
    /// </summary>
    protected virtual UserRole? Role => null;

    /// <inheritdoc />
    public async Task InitializeAsync(UserConnection userConnection, WsReq data)
    {
        if (Role is null)
        {
            await HandleAsync(userConnection, data);
            return;
        }

        if (userConnection.Token is null)
        {
            if (data.Route == WsRoute.Auth)
            {
                await HandleAsync(userConnection, data);
                return;
            }
        }

        await userConnection.IsAuthenticated(JwtService, Role);

        await HandleAsync(userConnection, data);
    }

    /// <inheritdoc />
    public abstract Task HandleAsync(UserConnection userConnection, WsReq data);
}
