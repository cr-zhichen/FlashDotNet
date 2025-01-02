using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Newtonsoft.Json;

namespace FlashDotNet.Converters;

/// <summary>
/// 自定义 JSON 转换器，用于将枚举类型序列化为其显示名称（Display Name），并反序列化回枚举值。
/// </summary>
public class JsonStringEnumDisplayConverter : JsonConverter
{
    /// <summary>
    /// 确定此转换器是否可以处理指定类型的对象。
    /// </summary>
    /// <param name="objectType">要检查的对象类型。</param>
    /// <returns>如果对象类型是枚举类型，则返回 true；否则返回 false。</returns>
    public override bool CanConvert(Type objectType)
    {
        // 如果是可空类型，获取其基础类型
        var type = Nullable.GetUnderlyingType(objectType) ?? objectType;
        return type.IsEnum;
    }

    /// <summary>
    /// 将枚举值序列化为其显示名称（Display Name）。
    /// </summary>
    /// <param name="writer">用于写入 JSON 的 JsonWriter。</param>
    /// <param name="value">要序列化的枚举值。</param>
    /// <param name="serializer">调用此方法的 JsonSerializer。</param>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var enumType = value.GetType();
        var enumName = System.Enum.GetName(enumType, value);
        if (enumName != null)
        {
            var field = enumType.GetField(enumName);
            var displayAttribute = field?.GetCustomAttribute<DisplayAttribute>();
            var displayName = displayAttribute != null ? displayAttribute.Name : enumName;
            writer.WriteValue(displayName);
            return;
        }

        throw new JsonSerializationException($"无法识别的枚举值: {value}");
    }

    /// <summary>
    /// 将显示名称（Display Name）反序列化为枚举值。
    /// </summary>
    /// <param name="reader">用于读取 JSON 的 JsonReader。</param>
    /// <param name="objectType">目标枚举类型。</param>
    /// <param name="existingValue">现有的对象值。</param>
    /// <param name="serializer">调用此方法的 JsonSerializer。</param>
    /// <returns>反序列化后的枚举值。</returns>
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        bool isNullable = Nullable.GetUnderlyingType(objectType) != null;
        var enumType = Nullable.GetUnderlyingType(objectType) ?? objectType;

        if (reader.TokenType == JsonToken.Null)
        {
            if (isNullable)
                return null;
            throw new JsonSerializationException($"无法将 null 转换为非空枚举类型 {enumType.Name}.");
        }

        if (reader.TokenType != JsonToken.String)
            throw new JsonSerializationException($"错误的令牌类型: {reader.TokenType}. 预期: String.");

        var displayName = reader.Value?.ToString();
        if (string.IsNullOrEmpty(displayName))
            throw new JsonSerializationException($"值为空，无法转换为枚举类型 {enumType.Name}.");

        foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var displayAttribute = field.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute?.Name != null && displayAttribute.Name.Equals(displayName, StringComparison.OrdinalIgnoreCase))
            {
                return System.Enum.Parse(enumType, field.Name);
            }

            if (field.Name.Equals(displayName, StringComparison.OrdinalIgnoreCase))
            {
                return System.Enum.Parse(enumType, field.Name);
            }
        }

        throw new JsonSerializationException($"无法将 '{displayName}' 转换为枚举类型 {enumType.Name}.");
    }
}
