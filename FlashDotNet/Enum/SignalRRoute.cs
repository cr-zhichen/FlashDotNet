using System.ComponentModel.DataAnnotations;

namespace FlashDotNet.Enum;

/// <summary>
/// SignalR传递的路由
/// </summary>
public enum SignalRRoute
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
    /// 鉴权
    /// </summary>
    [Display(Name = "auth", Description = "鉴权")]
    Auth,

    /// <summary>
    /// 广播消息
    /// </summary>
    [Display(Name = "broadcast_message", Description = "广播消息")]
    BroadcastMessage,
}
