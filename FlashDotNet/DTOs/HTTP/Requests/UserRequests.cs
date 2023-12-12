using FlashDotNet.Attribute;
using FlashDotNet.Enum;

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
    [PasswordValidation(ErrorMessage = "密码需要在数字、字母、特殊符号中三选二")]
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
    [PasswordValidation(ErrorMessage = "密码需要在数字、字母、特殊符号中三选二")]
    public string Password { get; set; } = "";
}