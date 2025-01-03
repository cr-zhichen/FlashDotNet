using FlashDotNet.Jwt;

namespace FlashDotNet.Utils;

/// <summary>
/// Token相关的工具类
/// </summary>
public static class TokenHelper
{
    /// <summary>
    /// 获取Token
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string GetToken(HttpRequest request)
        => request.Headers["Authorization"].ToString().Split(' ').Last();
}

/// <summary>
/// 用户相关的工具类
/// </summary>
public static class UserHelper
{
    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    /// <param name="jwtService"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public static async Task<Guid> GetCurrentUserIdAsync(IJwtService jwtService, HttpRequest request)
    {
        var token = TokenHelper.GetToken(request);
        var userInfo = await jwtService.GetUserInfoAsync(token);
        return userInfo?.UserId.ToGuid() ?? throw new UnauthorizedAccessException();
    }
}
