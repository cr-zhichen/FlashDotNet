using FlashDotNet.Attribute;
using FlashDotNet.DTOs;
using FlashDotNet.Enum;
using FlashDotNet.Infrastructure;
using FlashDotNet.Jwt;
using FlashDotNet.Services.CacheService;
using FlashDotNet.Utils;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace FlashDotNet.SignalR.Helper;

/// <summary>
/// SignalR Hub 处理器
/// </summary>
public abstract class HubHandler : Hub
{
    /// <summary>
    /// 缓存前缀
    /// </summary>
    public const string CachePrefix = "SignalR_Token_";

    /// <summary>
    /// JWT 服务
    /// </summary>
    public readonly IJwtService JwtService;

    /// <summary>
    /// 缓存服务
    /// </summary>
    public readonly ICacheService CacheService;

    private readonly ILogger<HubHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    protected HubHandler(ILogger<HubHandler> logger, IJwtService jwtService, ICacheService cacheService)
    {
        _logger = logger;
        JwtService = jwtService;
        CacheService = cacheService;
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    /// <param name="exception"></param>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        _logger.LogInformation($"[SignalR] 用户 {connectionId} 断开连接");
        CacheService.Remove(CachePrefix + connectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// 验证
    /// </summary>
    /// <param name="token"></param>
    [SignalRHubName(SignalRRoute.Auth)]
    public async Task Auth(string token)
    {
        _logger.LogInformation($"[SignalR] 用户 {Context.ConnectionId} 验证 Token");
        var (isValid, errorMessage) = await JwtService.ValidateTokenAsync(token);
        if (!isValid)
        {
            _logger.LogWarning($"[SignalR] 用户 {Context.ConnectionId} 验证失败：{errorMessage}");
            await SendAsync(SignalRRoute.Error, new Ok<object>()
            {
                Message = errorMessage ?? "验证失败"
            });
            Context.Abort();
        }

        _logger.LogInformation($"[SignalR] 用户 {Context.ConnectionId} 验证成功");
        CacheService.Set<string>(CachePrefix + Context.ConnectionId, token);
        await SendAsync(SignalRRoute.Auth, new Ok<object>()
        {
            Message = "验证成功"
        });
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="route">发送路由</param>
    /// <param name="obj">发送对象</param>
    protected internal async Task SendAsync<T>(SignalRRoute route, IRe<T> obj)
    {
        await Clients.Caller.SendAsync(route.GetDisplayName(), obj);
    }

    /// <summary>
    /// 发送消息给所有人
    /// </summary>
    /// <param name="route">发送路由</param>
    /// <param name="obj">发送对象</param>
    protected internal async Task SendAllAsync<T>(SignalRRoute route, IRe<T> obj)
    {
        await Clients.All.SendAsync(route.GetDisplayName(), obj);
    }

    /// <summary>
    /// 发送消息给指定用户
    /// </summary>
    /// <param name="route">发送路由</param>
    /// <param name="userId">用户ID</param>
    /// <param name="obj">发送对象</param>
    protected internal async Task SendUserAsync<T>(SignalRRoute route, string userId, IRe<T> obj)
    {
        await Clients.User(userId).SendAsync(route.GetDisplayName(), obj);
    }
}
