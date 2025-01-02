using System.ComponentModel.DataAnnotations;
using FlashDotNet.Utils;

namespace FlashDotNet.Attribute;

/// <summary>
/// 表示用于验证手机号码的属性。
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class PhoneNumberValidationAttribute : ValidationAttribute
{
    /// <summary>
    /// 检查给定值是否为有效的手机号码。
    /// </summary>
    /// <param name="value">要验证的值。应该是字符串类型的手机号码。</param>
    /// <returns>如果值是有效的手机号码，则为True，否则为False。</returns>
    public override bool IsValid(object? value)
    {
        string? phoneNumber = value as string;
        if (string.IsNullOrEmpty(phoneNumber))
        {
            return false;
        }

        return phoneNumber.IsMobile();
    }

    /// <summary>
    /// 重写默认的错误消息。
    /// </summary>
    /// <param name="name">属性名称。</param>
    /// <returns>错误消息。</returns>
    public override string FormatErrorMessage(string name)
    {
        return $"{name} 不是一个有效的手机号码。";
    }
}
