using System.ComponentModel.DataAnnotations;

namespace FlashDotNet.Enum;

/// <summary>
/// 缓存类型 
/// </summary>
public enum CacheType
{
    /// <summary>
    /// 内存缓存
    /// </summary>
    [Display(Name = "memory", Description = "内存缓存")]
    Memory,

    /// <summary>
    /// Redis缓存
    /// </summary>
    [Display(Name = "redis", Description = "Redis缓存")]
    Redis
}
