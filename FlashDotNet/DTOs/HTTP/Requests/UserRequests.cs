using FlashDotNet.Enum;
using FlashDotNet.Models;

namespace FlashDotNet.DTOs.HTTP.Requests;

/// <summary>
/// 注册
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = "";

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = "";

    /// <summary>
    /// 权限
    /// </summary>
    public UserRole Role { get; set; } = UserRole.User;
}

/// <summary>
/// 登录
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = "";

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = "";
}