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
public class TestRouteHandlers : WsRouteHandler<object>
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
    override protected async Task HandleAsync(UserConnection userConnection, object data)
    {
        await userConnection.SendMessageAsync(new WsOk<object>
        {
            Route = WsRoute.Test,
            Message = "test",
            Data = data
        });
    }
}
