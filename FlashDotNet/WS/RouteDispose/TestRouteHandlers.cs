using FlashDotNet.DTOs.WebSocket;
using FlashDotNet.Enum;
using FlashDotNet.Infrastructure;
using FlashDotNet.Jwt;
using FlashDotNet.WS.Helper;

namespace FlashDotNet.WS.RouteDispose;

/// <summary>
/// 测试路由处理器
/// </summary>
[AddScopedAsImplementedInterfaces]
public class TestRouteHandlers : WsRouteHandler
{
    /// <inheritdoc />
    public TestRouteHandlers(IJwtService jwtService) : base(jwtService)
    {
    }

    /// <inheritdoc />
    public override WsRoute Route => WsRoute.Test;

    /// <inheritdoc />
    override protected UserRole? Role => UserRole.Admin;

    /// <inheritdoc />
    public override async Task HandleAsync(UserConnection userConnection, WsReq data)
    {
        await userConnection.SendMessageAsync(new WsOk<object>
        {
            Route = WsRoute.Test,
            Message = "Test",
            Data = null
        });
    }
}
