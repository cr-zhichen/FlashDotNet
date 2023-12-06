using FlashDotNet.Enum;

namespace FlashDotNet.DTOs.HTTP.Responses;

/// <summary>
/// 用户注册 返回响应体
/// </summary>
public class RegisterResponse
{
    /// <summary>
    /// 用户名
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Username { get; set; } = null!;

    /// <summary>
    /// 用户角色
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Role { get; set; } = UserRole.User.ToString();

    /// <summary>
    /// 令牌
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Token { get; set; } = null!;
}

/// <summary>
/// 用户登录 返回响应体
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// 用户名
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Username { get; set; } = null!;

    /// <summary>
    /// 用户角色
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Role { get; set; } = null!;

    /// <summary>
    /// 令牌
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Token { get; set; } = null!;
}

/// <summary>
/// 获取用户列表 返回响应体
/// </summary>
public class GetUserListResponse
{
    /// <summary>
    /// 用户id
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Username { get; set; } = null!;

    /// <summary>
    /// 用户角色
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Role { get; set; } = null!;
}

/// <summary>
/// 获取用户信息 返回响应体
/// </summary>
public class GetUserInfoResponse
{
    /// <summary>
    /// 用户id
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Username { get; set; } = null!;

    /// <summary>
    /// 用户角色
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Role { get; set; } = null!;
}