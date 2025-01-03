using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashDotNet.Models;

/// <summary>
/// 实体基类，用于表示系统中的实体信息。
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// 创建时间 Utc时间
    /// </summary>
    [Required(ErrorMessage = "创建时间不能为空")]
    [Column("create_time")]
    public DateTime CreateTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间 Utc时间
    /// </summary>
    [Required(ErrorMessage = "更新时间不能为空")]
    [Column("update_time")]
    public required DateTime UpdateTime { get; set; }
}
