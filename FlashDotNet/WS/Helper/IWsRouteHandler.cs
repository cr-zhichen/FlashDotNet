using FlashDotNet.DTOs.WebSocket;
using FlashDotNet.Enum;
using FlashDotNet.Infrastructure;
using FlashDotNet.Jwt;
using FlashDotNet.Utils;
using Newtonsoft.Json;

namespace FlashDotNet.WS.Helper
{
    /// <summary>
    /// 定义WebSocket路由处理接口
    /// </summary>
    public interface IWsRouteHandler
    {
        /// <summary>
        /// 此处理器负责处理的路由枚举值
        /// </summary>
        public WsRoute Route { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="userConnection">用户连接包装对象</param>
        /// <param name="data">请求数据</param>
        /// <returns></returns>
        public Task InitializeAsync(UserConnection userConnection, WsReq data);
    }

    /// <inheritdoc />
    [AddScopedAsImplementedInterfaces]
    public abstract class WsRouteHandler<T> : IWsRouteHandler
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
        /// 此处理器负责处理角色鉴权
        /// </summary>
        protected virtual UserRole? Role => null;

        /// <inheritdoc />
        public async Task InitializeAsync(UserConnection userConnection, WsReq data)
        {
            T dataObject;
            try
            {
                // 根据 targetType 反序列化
                dataObject = JsonConvert.DeserializeObject<T>(data.Data?.ToString() ?? "", JsonConfigurationHelper.GetDefaultSettings()) ?? throw new ArgumentException("请求数据为空");
            }
            catch (JsonException ex)
            {
                // JsonException 可以更准确地捕获反序列化错误
                throw new ArgumentException($"请求数据反序列化异常: {ex.Message}", ex);
            }

            // 如果这个路由不需要校验角色，直接处理即可
            if (Role is null)
            {
                await HandleAsync(userConnection, dataObject);
                return;
            }

            // Auth 路由可以不需要先校验 Token
            if (data.Route == WsRoute.Auth)
            {
                await HandleAsync(userConnection, dataObject);
                return;
            }

            // 校验 Token
            await userConnection.IsAuthenticated(JwtService, Role);
            await HandleAsync(userConnection, dataObject);
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="userConnection"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract Task HandleAsync(UserConnection userConnection, T data);
    }
}
