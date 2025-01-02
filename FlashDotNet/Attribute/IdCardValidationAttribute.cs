using System.ComponentModel.DataAnnotations;
using FlashDotNet.Utils;

namespace FlashDotNet.Attribute;

/// <summary>
/// 身份证号码验证
/// </summary>
public class IdCardValidationAttribute : ValidationAttribute
{
    override protected ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is string idCard && IsValidIdCard(idCard))
        {
            return ValidationResult.Success!;
        }
        return new ValidationResult(ErrorMessage ?? "身份证号码格式不正确");
    }

    private bool IsValidIdCard(string idCard)
    {
        return idCard.IsIdCard();
    }
}