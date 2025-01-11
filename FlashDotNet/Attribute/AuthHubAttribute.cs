using FlashDotNet.Enum;
using FlashDotNet.SignalR.Helper;
using AspectCore.DynamicProxy;

namespace FlashDotNet.Attribute;

/// <summary>
/// SignalR Hub 验证
/// </summary>
[Serializable]
[AttributeUsage(AttributeTargets.Method)]
public class AuthHubAttribute : AbstractInterceptorAttribute
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public readonly UserRole RequiredRole;

    /// <summary>
    /// 验证JWT Token
    /// </summary>
    /// <param name="requiredRole">用户角色</param>
    public AuthHubAttribute(UserRole requiredRole)
    {
        RequiredRole = requiredRole;
    }

    /// <summary>
    /// 验证JWT Token，默认角色为None
    /// </summary>
    public AuthHubAttribute()
    {
        RequiredRole = UserRole.None;
    }

    /// <summary>
    /// 方法执行前后拦截
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public override async Task Invoke(AspectContext context, AspectDelegate next)
    {
        var hubHandlerInstance = context.Implementation as HubHandler;
        if (hubHandlerInstance == null)
        {
            throw new Exception("被AuthHubAttribute标记的方法必须在继承自HubHandler的类中");
        }

        var token = hubHandlerInstance.CacheService.Get<string>(HubHandler.CachePrefix + hubHandlerInstance.Context.ConnectionId);

        if (string.IsNullOrEmpty(token))
        {
            await hubHandlerInstance.SendAsync(SignalRRoute.Error, "未找到Token");
            hubHandlerInstance.Context.Abort();
            return;
        }

        var isValid = await hubHandlerInstance.JwtService.ValidateTokenAsync(token, RequiredRole);

        if (!isValid.IsValid)
        {
            await hubHandlerInstance.SendAsync(SignalRRoute.Error, isValid.ErrorMessage ?? "Token验证失败");
            hubHandlerInstance.Context.Abort();
            return;
        }

        await next(context); // 继续执行目标方法
    }
}
