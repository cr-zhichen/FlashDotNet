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
    [PasswordValidation(ErrorMessage = "密码必须包含至少一个大写字母、一个小写字母、一个数字和一个特殊字符")]
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
    [PasswordValidation(ErrorMessage = "密码必须包含至少一个大写字母、一个小写字母、一个数字和一个特殊字符")]
    public string Password { get; set; } = "";
}