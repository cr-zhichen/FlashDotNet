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
    /// 主键 ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("user_id")]
    [Display(Name = "用户ID")]
    public int UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("username")]
    [Display(Name = "用户名")]
    public string Username { get; set; } = null!;

    /// <summary>
    /// 密码
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("password_hash")]
    [Display(Name = "密码")]
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// 权限
    /// </summary>
    [Required]
    [Column("role")]
    [Display(Name = "权限")]
    public UserRole Role { get; set; } = UserRole.User;
}
