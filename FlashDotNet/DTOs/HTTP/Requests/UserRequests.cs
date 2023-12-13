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
    [PasswordValidation(ErrorMessage = "密码应由以下三种字符类型中的任意两种组成：数字、字母、特殊符号")]
    public string Password { get; set; } = "";
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
    [PasswordValidation(ErrorMessage = "密码应由以下三种字符类型中的任意两种组成：数字、字母、特殊符号")]
    public string Password { get; set; } = "";
}