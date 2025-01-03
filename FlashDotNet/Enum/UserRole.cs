using System.ComponentModel.DataAnnotations;

namespace FlashDotNet.Enum;

/// <summary>
/// 用户角色
/// </summary>
[Flags]
public enum UserRole
{
    /// <summary>
    /// 无角色 (游客)
    /// </summary>
    [Display(Name = "none", Description = "游客")]
    None = 0,

    /// <summary>
    /// 基础角色 (用户)
    /// </summary>
    [Display(Name = "base", Description = "用户")]
    Base = None | (1 << 0),

    /// <summary>
    /// 管理员 (可以管理用户和内容，但不能修改系统设置)
    /// </summary>
    [Display(Name = "admin", Description = "管理员")]
    Admin = Base | (1 << 4),

    /// <summary>
    /// 超级管理员 (拥有所有权限)
    /// </summary>
    [Display(Name = "super_admin", Description = "超级管理员")]
    SuperAdmin = Admin | (1 << 8),
}
