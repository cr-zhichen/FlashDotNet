using System.ComponentModel.DataAnnotations;

namespace FlashDotNet.Enum;

/// <summary>
/// Ws传递的路由
/// </summary>
public enum WsRoute
{
    /// <summary>
    /// 失败
    /// </summary>
    [Display(Name = "error", Description = "失败")]
    Error,

    /// <summary>
    /// 测试
    /// </summary>
    [Display(Name = "test", Description = "测试")]
    Test,

    /// <summary>
    /// 首次连接
    /// </summary>
    [Display(Name = "first", Description = "首次连接")]
    First,

    /// <summary>
    /// 鉴权
    /// </summary>
    [Display(Name = "Auth", Description = "鉴权")]
    Auth,
}
