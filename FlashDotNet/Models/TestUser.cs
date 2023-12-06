using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlashDotNet.Enum;

namespace FlashDotNet.Models;

/// <summary>
/// 表示数据库中的TestDataBase表。
/// </summary>
[Table("TestUser")]
public class TestUser
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [Key]
    public int UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = "";

    /// <summary>
    /// 密码
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Password { get; set; } = "";

    /// <summary>
    /// 权限
    /// </summary>
    [Required]
    public UserRole Role { get; set; } = UserRole.User;
}