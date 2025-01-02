using System.ComponentModel.DataAnnotations;

namespace FlashDotNet.Attribute;

/// <summary>
/// 验证上传文件的 MIME 类型
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class AllowedFileTypesAttribute : ValidationAttribute
{
    private readonly string[] _allowedMimeTypes;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="allowedMimeTypes">允许的 MIME 类型，如 "video/mp4", "application/pdf"</param>
    public AllowedFileTypesAttribute(string[] allowedMimeTypes)
    {
        _allowedMimeTypes = allowedMimeTypes.Select(m => m.ToLower()).ToArray();
    }

    override protected ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IFormFile file)
        {
            // 如果文件为空，由 [Required] 属性处理
            return ValidationResult.Success;
        }

        var mimeType = file.ContentType.ToLower();
        if (!_allowedMimeTypes.Contains(mimeType))
        {
            return new ValidationResult($"不允许的文件类型，仅限: {string.Join(", ", _allowedMimeTypes)}");
        }

        return ValidationResult.Success;
    }
}
