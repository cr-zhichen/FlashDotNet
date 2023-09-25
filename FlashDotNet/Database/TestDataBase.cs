using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashDotNet.Database;

[Table("TestDataBase")]
public class TestDataBase
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 自增长
    public int Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [Required] // NOT NULL
    [StringLength(100)] // 字符串最大长度
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

    /// <summary>
    /// 用户角色
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// 管理员
        /// </summary>
        Admin,

        /// <summary>
        /// 用户
        /// </summary>
        User,

        /// <summary>
        /// 游客
        /// </summary>
        Guest
    }
}