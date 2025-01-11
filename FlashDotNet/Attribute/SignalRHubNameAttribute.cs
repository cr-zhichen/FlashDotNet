using FlashDotNet.Enum;
using FlashDotNet.Utils;
using Microsoft.AspNetCore.SignalR;

namespace FlashDotNet.Attribute;

/// <inheritdoc />
public class SignalRHubNameAttribute : HubMethodNameAttribute
{
    /// <summary>
    /// 设置SignalR的Hub名称
    /// </summary>
    /// <param name="route">路由</param>
    public SignalRHubNameAttribute(SignalRRoute route) : base(route.GetDisplayName())
    {
    }
}
