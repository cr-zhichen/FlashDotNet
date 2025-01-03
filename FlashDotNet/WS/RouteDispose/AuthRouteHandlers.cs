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
public class AuthRouteHandlers : WsRouteHandler
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
    public override async Task HandleAsync(UserConnection userConnection, WsReq data)
    {
        AuthReq authReq;

        try
        {
            authReq = JsonConvert.DeserializeObject<AuthReq>(data.Data?.ToString() ?? "") ?? throw new InvalidOperationException();
        }
        catch (Exception)
        {
            throw new ArgumentException("请求数据格式错误");
        }

        userConnection.Token = authReq.Token;

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
