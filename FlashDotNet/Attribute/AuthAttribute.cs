using FlashDotNet.DTOs;
using FlashDotNet.Enum;
using FlashDotNet.Jwt;
using FlashDotNet.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FlashDotNet.Attribute;

/// <summary>
/// 验证JWT Token
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthAttribute : ActionFilterAttribute
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public readonly UserRole RequiredRole;

    /// <summary>
    /// 验证JWT Token
    /// </summary>
    /// <param name="requiredRole">用户角色</param>
    public AuthAttribute(UserRole requiredRole)
    {
        RequiredRole = requiredRole;
    }

    /// <summary>
    /// 验证JWT Token
    /// </summary>
    public AuthAttribute()
    {
        RequiredRole = UserRole.None;
    }

    /// <summary>
    /// 验证JWT Token
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var token = context.HttpContext.Request.Headers["Authorization"].ToString().Split(' ').Last();

        // 使用服务定位器来获取 IJwtService
        var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();

        var isValid = await jwtService!.ValidateTokenAsync(token, RequiredRole);

        if (!isValid.IsValid)
        {
            var errorObject = new Error<object>
            {
                Code = Code.TokenError,
                Message = "token验证失败",
                Data = isValid.ErrorMessage
            };

            context.Result = new JsonResult(errorObject, JsonConfigurationHelper.GetDefaultSettings()) { StatusCode = 200 };
            return;
        }

        await next();
    }
}
