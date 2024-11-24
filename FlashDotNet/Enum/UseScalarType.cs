using System.ComponentModel.DataAnnotations;

namespace FlashDotNet.Enum;

/// <summary>
/// Scalar的使用情况
/// </summary>
public enum UseScalarType
{
    /// <summary>
    /// 使用Scalar
    /// </summary>
    [Display(Name = "true", Description = "使用Scalar")]
    True,

    /// <summary>
    /// 不使用Scalar
    /// </summary>
    [Display(Name = "false", Description = "不使用Scalar")]
    False,

    /// <summary>
    /// 自动判断是否使用Scalar
    /// </summary>
    [Display(Name = "auto", Description = "自动判断是否使用Scalar")]
    Auto,
}
