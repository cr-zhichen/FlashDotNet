using FlashDotNet.DTOs.WebSocket;
using FlashDotNet.Enum;
using FlashDotNet.Infrastructure;
using FlashDotNet.Jwt;
using FlashDotNet.WS.Helper;
using Newtonsoft.Json;

namespace FlashDotNet.WS.RouteDispose;

/// <summary>
/// 认证路由处理器
/// </summary>
[AddScopedAsImplementedInterfaces]
public class AuthRouteHandlers : WsRouteHandler<AuthRouteHandlers.AuthReq>
{
    /// <inheritdoc />
    public AuthRouteHandlers(IJwtService jwtService) : base(jwtService)
    {
    }

    /// <inheritdoc />
    public override WsRoute Route => WsRoute.Auth;

    /// <inheritdoc />
    override protected UserRole? Role => null;

    /// <inheritdoc />
    override protected async Task HandleAsync(UserConnection userConnection, AuthReq data)
    {
        userConnection.Token = data.Token;

        await userConnection.IsAuthenticated(JwtService);

        await userConnection.SendMessageAsync(new WsOk<object>
        {
            Route = WsRoute.Auth,
            Message = "认证成功",
            Data = null
        });
    }

    /// <summary>
    /// 认证请求
    /// </summary>
    public class AuthReq
    {
        /// <summary>
        /// Token
        /// </summary>
        public required string Token { get; set; }
    }
}
