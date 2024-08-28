using System.ComponentModel.DataAnnotations;

namespace FlashDotNet.Enum;

public enum Code
{
    /// <summary>
    /// 未知错误
    /// </summary>
    [Display(Name = "error", Description = "未知错误")]
    Error = -1,

    /// <summary>
    /// 成功
    /// </summary>
    [Display(Name = "success", Description = "成功")]
    Success = 0,

    /// <summary>
    /// Token错误
    /// </summary>
    [Display(Name = "token_error", Description = "Token错误")]
    TokenError = 1,
}
