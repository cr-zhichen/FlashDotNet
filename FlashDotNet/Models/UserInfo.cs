using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlashDotNet.Enum;

namespace FlashDotNet.Models;

/// <summary>
/// 表示数据库中的user_info表。
/// </summary>
[Table("user_info")]
public class UserInfo
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [Key]
    [Column("user_id")]
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public int UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("username")]
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public string Username { get; set; } = null!;

    /// <summary>
    /// 密码
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("password")]
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public string Password { get; set; } = null!;

    /// <summary>
    /// 权限
    /// </summary>
    [Required]
    [Column("role")]
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public UserRole Role { get; set; } = UserRole.User;
}