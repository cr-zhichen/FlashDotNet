using System.Reflection;
using FlashDotNet.Attribute;
using FlashDotNet.DTOs;
using FlashDotNet.Enum;
using FlashDotNet.Jwt;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace FlashDotNet.Filter;

public class AuthHubFilter : IHubFilter
{
    public async ValueTask<object> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object>> next)
    {
        // 获取当前调用的方法信息
        var methodInfo = invocationContext.HubMethod;

        // 获取方法上的 AuthAttribute
        var authAttribute = methodInfo.GetCustomAttribute<AuthAttribute>();

        if (authAttribute != null)
        {
            string token = null;
            var httpContext = invocationContext.Context.GetHttpContext();

            // 尝试从 Authorization 头中获取 Token
            if (httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = httpContext.Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer "))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();
                }
            }

            // 如果头部没有 Token，则尝试从查询字符串中获取
            if (string.IsNullOrEmpty(token) && httpContext.Request.Query.ContainsKey("access_token"))
            {
                token = httpContext.Request.Query["access_token"];
            }

            if (string.IsNullOrEmpty(token))
            {
                // Token 不存在，返回错误
                var errorObject = new Error<object>
                {
                    Code = Code.TokenError,
                    Message = "未提供 Token",
                    Data = "Token 是必需的"
                };
                throw new HubException(JsonConvert.SerializeObject(errorObject));
            }

            // 使用服务定位器获取 IJwtService
            var jwtService = httpContext.RequestServices.GetService<IJwtService>();

            if (jwtService == null)
            {
                var errorObject = new Error<object>
                {
                    Code = Code.Error,
                    Message = "服务器配置错误",
                    Data = "无法获取 IJwtService 服务"
                };
                throw new HubException(JsonConvert.SerializeObject(errorObject));
            }

            // 验证 Token
            var isValid = await jwtService.ValidateTokenAsync(token, authAttribute.RequiredRole);

            if (!isValid.IsValid)
            {
                // 返回错误信息
                var errorObject = new Error<object>
                {
                    Code = Code.TokenError,
                    Message = "Token 验证失败",
                    Data = isValid.ErrorMessage
                };

                // 返回错误响应
                throw new HubException(JsonConvert.SerializeObject(errorObject));
            }
        }

        // 继续执行下一个过滤器或实际的 Hub 方法
        return await next(invocationContext);
    }
}
