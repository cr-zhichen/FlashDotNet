using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlashDotNet.Enum;
using Microsoft.EntityFrameworkCore;

namespace FlashDotNet.Models;

/// <summary>
/// 用户实体类，表示系统中的用户。
/// </summary>
[Table("user")]
[Index(nameof(Username), IsUnique = true)]
public class User
{
    /// <summary>
    /// 用户唯一标识ID
    /// </summary>
    [Key]
    [Column("user_id")]
    public Guid UserId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, ErrorMessage = "用户名长度不能超过50个字符")]
    [Column("username")]
    public required string Username { get; set; }

    /// <summary>
    /// 密码哈希值
    /// </summary>
    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(128, ErrorMessage = "密码哈希长度不能超过128个字符")]
    [Column("password_hash")]
    public required string PasswordHash { get; set; }

    /// <summary>
    /// 用户角色类型
    /// </summary>
    [Required(ErrorMessage = "角色类型不能为空")]
    [Column("role_type")]
    public required UserRole RoleType { get; set; }

    /// <summary>
    /// Token版本号
    /// </summary>
    [Required(ErrorMessage = "Token版本号不能为空")]
    [Column("token_version")]
    public required Guid TokenVersion { get; set; }
}
