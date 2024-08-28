using System.ComponentModel.DataAnnotations;

namespace FlashDotNet.Enum;

/// <summary>
/// 用户角色
/// </summary>
public enum UserRole
{
    /// <summary>
    /// 管理员
    /// </summary>
    [Display(Name = "admin", Description = "管理员")]
    Admin,

    /// <summary>
    /// 用户
    /// </summary>
    [Display(Name = "user", Description = "用户")]
    User,

    /// <summary>
    /// 游客
    /// </summary>
    [Display(Name = "guest", Description = "游客")]
    Guest
}