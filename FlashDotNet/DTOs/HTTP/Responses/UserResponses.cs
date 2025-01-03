using FlashDotNet.Enum;

namespace FlashDotNet.DTOs.HTTP.Responses;

/// <summary>
/// 用户注册 返回响应体
/// </summary>
public class RegisterResponse
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string Username { get; set; } = null!;

    /// <summary>
    /// 用户角色
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required UserRole Role { get; set; } = UserRole.User;

    /// <summary>
    /// 令牌
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string Token { get; set; } = null!;
}

/// <summary>
/// 用户登录 返回响应体
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string Username { get; set; } = null!;

    /// <summary>
    /// 用户角色
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required UserRole Role { get; set; } = UserRole.User;

    /// <summary>
    /// 令牌
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string Token { get; set; } = null!;
}

/// <summary>
/// 获取用户列表 返回响应体
/// </summary>
public class GetUserListResponse
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string Username { get; set; } = null!;

    /// <summary>
    /// 用户角色
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required UserRole Role { get; set; } = UserRole.User;
}

/// <summary>
/// 获取用户信息 返回响应体
/// </summary>
public class GetUserInfoResponse
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string Username { get; set; } = null!;

    /// <summary>
    /// 用户角色
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required UserRole Role { get; set; } = UserRole.User;
}
