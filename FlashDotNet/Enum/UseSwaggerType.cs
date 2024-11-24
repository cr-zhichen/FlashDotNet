using System.ComponentModel.DataAnnotations;
namespace FlashDotNet.Enum;

/// <summary>
/// Swagger的使用情况
/// </summary>
public enum UseSwaggerType
{
    /// <summary>
    /// 使用Swagger
    /// </summary>
    [Display(Name = "true", Description = "使用Swagger")]
    True,

    /// <summary>
    /// 不使用Swagger
    /// </summary>
    [Display(Name = "false", Description = "不使用Swagger")]
    False,

    /// <summary>
    /// 自动判断是否使用Swagger
    /// </summary>
    [Display(Name = "auto", Description = "自动判断是否使用Swagger")]
    Auto,
}
