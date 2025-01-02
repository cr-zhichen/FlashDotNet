using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Newtonsoft.Json;

namespace FlashDotNet.Converters;

/// <summary>
/// 自定义 JSON 转换器，用于将枚举类型与其显示名称（通过 <see cref="DisplayAttribute"/> 指定）进行序列化和反序列化。
/// </summary>
/// <typeparam name="T">枚举类型，必须为值类型且继承自 <see cref="System.Enum"/>。</typeparam>
public class JsonStringEnumDisplayConverter<T> : JsonConverter where T:struct, System.Enum
{
    /// <summary>
    /// 将枚举值序列化为其显示名称。
    /// </summary>
    /// <param name="writer">用于写入 JSON 数据的 <see cref="JsonWriter"/>。</param>
    /// <param name="value">要序列化的枚举值。</param>
    /// <param name="serializer">用于序列化的 <see cref="JsonSerializer"/>。</param>
    /// <exception cref="JsonSerializationException">当传入的值不是有效的枚举值时抛出。</exception>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is T enumValue)
        {
            var enumType = typeof(T);
            var field = enumType.GetField(enumValue.ToString());

            // 获取枚举字段的 DisplayAttribute 特性
            var displayAttribute = field?.GetCustomAttribute<DisplayAttribute>();
            // 如果存在 DisplayAttribute，则使用其 Name 属性作为显示名称，否则使用枚举值的字符串表示
            var displayName = displayAttribute != null ? displayAttribute.Name : enumValue.ToString();

            writer.WriteValue(displayName);
        }
        else
        {
            throw new JsonSerializationException("无效的枚举值");
        }
    }

    /// <summary>
    /// 将显示名称反序列化为枚举值。
    /// </summary>
    /// <param name="reader">用于读取 JSON 数据的 <see cref="JsonReader"/>。</param>
    /// <param name="objectType">目标对象的类型。</param>
    /// <param name="existingValue">现有的对象值。</param>
    /// <param name="serializer">用于反序列化的 <see cref="JsonSerializer"/>。</param>
    /// <returns>反序列化后的枚举值。</returns>
    /// <exception cref="JsonSerializationException">当显示名称为空或无法转换为枚举值时抛出。</exception>
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var displayName = reader.Value?.ToString();
        if (displayName == null)
            throw new JsonSerializationException($"值为空，无法转换为 {typeof(T).Name}。");

        // 遍历枚举类型的所有字段，查找与显示名称匹配的枚举值
        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var displayAttribute = field.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null && displayAttribute.Name.Equals(displayName, StringComparison.OrdinalIgnoreCase))
            {
                return (T)field.GetValue(null)!;
            }
        }

        throw new JsonSerializationException($"无法将 '{displayName}' 转换为枚举类型 {typeof(T).Name}。");
    }

    /// <summary>
    /// 确定当前转换器是否可以处理指定的类型。
    /// </summary>
    /// <param name="objectType">要检查的类型。</param>
    /// <returns>如果类型为 <typeparamref name="T"/>，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(T);
    }
}
