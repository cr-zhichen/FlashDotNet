using System.ComponentModel.DataAnnotations;

namespace FlashDotNet.Attribute;

/// <summary>
/// 表示用于验证密码的属性。
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class PasswordValidationAttribute : ValidationAttribute
{
    ///<summary>
    ///检查给定值是否为有效密码。
    ///</summary>
    ///<param name=“value”>要验证的值。应该是字符串</param>
    ///<returns>如果值是有效密码，则为True，否则为false</returns>
    public override bool IsValid(object value)
    {
        string password = value as string;
        if (string.IsNullOrEmpty(password))
        {
            return false;
        }

        // 正则表达式规则：密码需要在数字、字母、特殊符号中三选二
        // (?=.*\d)：包含至少一个数字
        // (?=.*[a-zA-Z])：包含至少一个字母
        // (?=.*[\W_])：包含至少一个特殊符号
        // ^(?=.*\d)(?=.*[a-zA-Z])|(?=.*\d)(?=.*[\W_])|(?=.*[a-zA-Z])(?=.*[\W_]).{8,}$：三选二规则，最少8位长度
        string regexPattern = @"^(?=.*\d)(?=.*[a-zA-Z])|(?=.*\d)(?=.*[\W_])|(?=.*[a-zA-Z])(?=.*[\W_]).{8,}$";

        return System.Text.RegularExpressions.Regex.IsMatch(password, regexPattern);
    }
}