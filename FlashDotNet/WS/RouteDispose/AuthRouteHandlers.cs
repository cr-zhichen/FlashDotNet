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
public class AuthRouteHandlers : IWsRouteHandler
{
    private readonly IJwtService _jwtService;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AuthRouteHandlers(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    /// <inheritdoc />
    public WsRoute Route => WsRoute.Auth;

    /// <inheritdoc />
    public async Task HandleAsync(UserConnection userConnection, WsReq data)
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

        await userConnection.IsAuthenticated(_jwtService);

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
