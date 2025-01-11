using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.SignalR;

namespace FlashDotNet.Utils;

/// <summary>
/// 扩展方法
/// </summary>
public static class Expand
{
    # region 链式调用

    /// <summary>
    /// 链式调用
    /// </summary>
    /// <param name="t">自身传递</param>
    /// <param name="action">委托调用</param>
    /// <typeparam name="T">自身类</typeparam>
    /// <returns>自身传出</returns>
    public static T Do<T>(this T t, Action<T> action)
    {
        action(t);
        return t;
    }

    #endregion

    #region Argon2计算

    /// <summary>
    /// Argon2计算
    /// </summary>
    /// <param name="str"></param>
    /// <param name="salt"></param>
    /// <returns></returns>
    public static string ToArgon2(this string str, string? salt = null)
    {
        using var hasher = new Argon2id(Encoding.UTF8.GetBytes(str));
        if (!string.IsNullOrEmpty(salt))
        {
            hasher.Salt = Encoding.UTF8.GetBytes(salt);
        }

        // 参数调整
        hasher.DegreeOfParallelism = 4; // 减少线程数，减轻CPU负担
        hasher.MemorySize = 32768; // 降低内存使用量到32 MB
        hasher.Iterations = 3; // 稍微减少迭代次数

        var hashBytes = hasher.GetBytes(32); // 获取32字节的哈希值
        return Convert.ToBase64String(hashBytes);
    }

    #endregion

    #region md5计算

    /// <summary>
    /// 计算md5值
    /// </summary>
    /// <param name="str">自身传递</param>
    /// <param name="toUpper">是否为大写</param>
    /// <param name="to16">是否为16位md5</param>
    /// <returns>md5</returns>
    public static string ToMd5(this string str, bool toUpper = false, bool to16 = false)
    {
        //将str进行MD5加密
        var md5 = MD5.Create();
        var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
        var sb = new StringBuilder();
        foreach (var b in bs)
        {
            sb.Append(b.ToString(toUpper ? "X2" : "x2"));
        }

        return to16 ? sb.ToString().Substring(8, 16) : sb.ToString();
    }

    /// <summary>
    /// 计算md5值
    /// </summary>
    /// <param name="bytes">自身传递</param>
    /// <param name="toUpper">是否为大写</param>
    /// <param name="to16">是否为16位md5</param>
    /// <returns>md5</returns>
    public static string ToMd5(this byte[] bytes, bool toUpper = false, bool to16 = false)
    {
        var md5 = MD5.Create();
        var bs = md5.ComputeHash(bytes);
        var sb = new StringBuilder();
        foreach (var b in bs)
        {
            sb.Append(b.ToString(toUpper ? "X2" : "x2"));
        }

        return to16 ? sb.ToString().Substring(8, 16) : sb.ToString();
    }

    #endregion

    #region 字符串转换

    /// <summary>
    /// string转int
    /// </summary>
    /// <param name="str">自身传递</param>
    /// <returns>int值</returns>
    /// <exception cref="Exception">转换失败</exception>
    public static int ToInt(this string str)
    {
        return int.TryParse(str, out var result) ? result : throw new Exception("string转换为int失败");
    }

    /// <summary>
    /// string转float
    /// </summary>
    /// <param name="str">自身传递</param>
    /// <returns>float值</returns>
    /// <exception cref="Exception">转换失败</exception>
    public static float ToFloat(this string str)
    {
        return float.TryParse(str, out var result) ? result : throw new Exception("string转换为float失败");
    }

    /// <summary>
    /// string转double
    /// </summary>
    /// <param name="str">自身传递</param>
    /// <returns>double值</returns>
    /// <exception cref="Exception">转换失败</exception>
    public static double ToDouble(this string str)
    {
        return double.TryParse(str, out var result) ? result : throw new Exception("string转换为double失败");
    }

    /// <summary>
    /// string 转Guid
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Guid ToGuid(this string str)
    {
        return Guid.TryParse(str, out var result) ? result : throw new Exception("string转换为Guid失败");
    }

    #endregion

    #region 字符串格式判断

    /// <summary>
    /// 判断是否是Email格式
    /// </summary>
    /// <param name="str">自身传递</param>
    /// <returns></returns>
    public static bool IsEmail(this string str)
    {
        return Regex.IsMatch(str,
            @"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
    }

    /// <summary>
    /// 判断是否是手机号格式
    /// </summary>
    /// <param name="str">自身传递</param>
    /// <returns></returns>
    public static bool IsMobile(this string str)
    {
        return Regex.IsMatch(str, @"^1[3456789]\d{9}$");
    }

    /// <summary>
    /// 判断是否是身份证号格式
    /// </summary>
    /// <param name="str">自身传递</param>
    /// <returns></returns>
    public static bool IsIdCard(this string str)
    {
        return Regex.IsMatch(str,
            @"^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$|^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}([0-9]|X|x)$");
    }

    /// <summary>
    /// 判断是否是Url格式
    /// </summary>
    /// <param name="str">自身传递</param>
    /// <returns></returns>
    public static bool IsUrl(this string str)
    {
        return Regex.IsMatch(str,
            @"^(http|https|ftp|rtsp|mms):(\/\/|\\\\)[A-Za-z0-9%\-_@]+\.[A-Za-z0-9%\-_@]+[A-Za-z0-9\.\/=\?%\-&_~`@:\+!;]*$");
    }

    /// <summary>
    /// 判断是否是ipv4格式
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsIpv4(this string str)
    {
        return Regex.IsMatch(str, @"^((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$");
    }

    /// <summary>
    /// 判断是否是ipv6格式
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsIpv6(this string str)
    {
        return Regex.IsMatch(str,
            @"^([\da-fA-F]{1,4}:){6}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^::([\da-fA-F]{1,4}:){0,4}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:):([\da-fA-F]{1,4}:){0,3}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){2}:([\da-fA-F]{1,4}:){0,2}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){3}:([\da-fA-F]{1,4}:){0,1}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){4}:((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){7}[\da-fA-F]{1,4}$|^:((:[\da-fA-F]{1,4}){1,6}|:)$|^[\da-fA-F]{1,4}:((:[\da-fA-F]{1,4}){1,5}|:)$|^([\da-fA-F]{1,4}:){2}((:[\da-fA-F]{1,4}){1,4}|:)$|^([\da-fA-F]{1,4}:){3}((:[\da-fA-F]{1,4}){1,3}|:)$|^([\da-fA-F]{1,4}:){4}((:[\da-fA-F]{1,4}){1,2}|:)$|^([\da-fA-F]{1,4}:){5}:([\da-fA-F]{1,4})?$|^([\da-fA-F]{1,4}:){6}:$");
    }

    /// <summary>
    /// 判断是否是ip格式
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsIp(this string str)
    {
        return Regex.IsMatch(str,
            @"^((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){6}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^::([\da-fA-F]{1,4}:){0,4}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:):([\da-fA-F]{1,4}:){0,3}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){2}:([\da-fA-F]{1,4}:){0,2}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){3}:([\da-fA-F]{1,4}:){0,1}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){4}:((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){7}[\da-fA-F]{1,4}$|^:((:[\da-fA-F]{1,4}){1,6}|:)$|^[\da-fA-F]{1,4}:((:[\da-fA-F]{1,4}){1,5}|:)$|^([\da-fA-F]{1,4}:){2}((:[\da-fA-F]{1,4}){1,4}|:)$|^([\da-fA-F]{1,4}:){3}((:[\da-fA-F]{1,4}){1,3}|:)$|^([\da-fA-F]{1,4}:){4}((:[\da-fA-F]{1,4}){1,2}|:)$|^([\da-fA-F]{1,4}:){5}:([\da-fA-F]{1,4})?$|^([\da-fA-F]{1,4}:){6}:$");
    }

    /// <summary>
    /// 判断是否包含中文
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsChinese(this string str)
    {
        return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
    }

    /// <summary>
    /// 判断是否是中文姓名格式
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsChineseName(this string str)
    {
        return Regex.IsMatch(str, @"^[\u4e00-\u9fa5]{2,4}$");
    }

    /// <summary>
    /// 判断是否是英文
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsEnglish(this string str)
    {
        return Regex.IsMatch(str, @"^[A-Za-z]+$");
    }

    /// <summary>
    /// 判断是否是大写英文
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsUpperEnglish(this string str)
    {
        return Regex.IsMatch(str, @"^[A-Z]+$");
    }

    /// <summary>
    /// 判断是否是小写英文
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsLowerEnglish(this string str)
    {
        return Regex.IsMatch(str, @"^[a-z]+$");
    }

    #endregion

    #region 敏感信息处理

    /// <summary>
    /// 身份证号码脱敏
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string MaskIdCard(this string str)
    {
        return str.IsIdCard()
            ? str.Substring(0, 6) + "********" + str.Substring(14, 4)
            : throw new Exception("不是有效的身份证号码");
    }

    /// <summary>
    /// 手机号码脱敏
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string MaskMobile(this string str)
    {
        return str.IsMobile()
            ? str.Substring(0, 3) + "****" + str.Substring(7, 4)
            : throw new Exception("不是有效的手机号码");
    }

    /// <summary>
    /// 邮箱脱敏
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string MaskEmail(this string str)
    {
        return str.IsEmail()
            ? str.Substring(0, 3) + "****" + str.Substring(str.IndexOf('@'))
            : throw new Exception("不是有效的邮箱地址");
    }

    #endregion

    #region 时间戳转换

    /// <summary>
    /// 获取毫秒级时间戳
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static double GetTotalMilliseconds(this DateTime dt)
    {
        return (dt - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }

    /// <summary>
    /// 获取秒级时间戳
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static double GetTotalSeconds(this DateTime dt)
    {
        return (dt - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    /// <summary>
    /// 毫秒级时间戳转换为DateTime
    /// </summary>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static DateTime ToDateTimeMilliseconds(this double timestamp)
    {
        return new DateTime(1970, 1, 1).AddMilliseconds(timestamp);
    }

    /// <summary>
    /// 秒级时间戳转换为DateTime
    /// </summary>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static DateTime ToDateTimeSeconds(this double timestamp)
    {
        return new DateTime(1970, 1, 1).AddSeconds(timestamp);
    }

    /// <summary>
    /// 获取时间差
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="dt2">传入时间</param>
    /// <returns>返回时间差 小于0为在之前，大于0为在之后 </returns>
    public static TimeSpan GetTimeDifference(this DateTime dt, DateTime dt2 = default)
    {
        return dt - (dt2 == default(DateTime) ? DateTime.UtcNow : dt2);
    }

    #endregion

    #region 枚举相关

    /// <summary>
    /// 获取枚举值的 DisplayAttribute 特性名称；如果未使用该特性，则返回枚举的名称。
    /// </summary>
    /// <param name="enumValue">枚举值。</param>
    /// <returns>特性名称或枚举的名称。</returns>
    /// <exception cref="ArgumentNullException">当传入的 enumValue 为空时抛出。</exception>
    public static string GetDisplayName(this System.Enum enumValue)
    {
        if (enumValue == null)
            throw new ArgumentNullException(nameof(enumValue), "enumValue 不能为空。");

        FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo == null)
            throw new ArgumentException("无效的枚举值。", nameof(enumValue));

        return (fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false)
                   is DisplayAttribute[] { Length: > 0 } attrs
                   ? attrs[0].Name
                   : enumValue.ToString())
               ?? throw new Exception("获取枚举值的 DisplayAttribute 特性名称失败。");
    }

    /// <summary>
    /// 获取枚举值的 DisplayAttribute 特性说明；如果未使用该特性，则返回枚举的名称。
    /// </summary>
    /// <param name="enumValue">枚举值。</param>
    /// <returns>特性说明或枚举的名称。</returns>
    /// <exception cref="ArgumentNullException">当传入的 enumValue 为空时抛出。</exception>
    public static string GetDisplayDescription(this System.Enum enumValue)
    {
        if (enumValue == null)
            throw new ArgumentNullException(nameof(enumValue), "enumValue 不能为空。");

        FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo == null)
            throw new ArgumentException("无效的枚举值。", nameof(enumValue));

        return (fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false)
                   is DisplayAttribute[] { Length: > 0 } attrs
                   ? attrs[0].Description
                   : enumValue.ToString())
               ?? throw new Exception("获取枚举值的 DisplayAttribute 特性说明失败。");
    }

    /// <summary>
    /// 将字符串转换为枚举值，转换失败时返回默认值。
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <param name="enumString">要转换的字符串</param>
    /// <param name="ignoreCase">是否忽略大小写</param>
    /// <returns>转换后的枚举值，如果转换失败则返回默认值</returns>
    public static TEnum ToEnum<TEnum>(this string enumString, bool ignoreCase = true) where TEnum:struct
    {
        if (System.Enum.TryParse(enumString, ignoreCase, out TEnum result))
        {
            return result;
        }

        throw new ArgumentException($"无法将字符串 '{enumString}' 转换为枚举类型 {typeof(TEnum).Name}。");
    }

    private readonly static ConcurrentDictionary<Type, Dictionary<string, object?>> DisplayNameMap =
        new ConcurrentDictionary<Type, Dictionary<string, object?>>();

    private readonly static ConcurrentDictionary<Type, Dictionary<string, object?>> DisplayDescriptionMap =
        new ConcurrentDictionary<Type, Dictionary<string, object?>>();

    /// <summary>
    /// 根据 <see cref="DisplayAttribute"/> 的 Name 或 Description 将字符串转换为对应的枚举值。
    /// </summary>
    /// <typeparam name="TEnum">要转换的枚举类型。此类型必须是一个枚举。</typeparam>
    /// <param name="displayString">
    /// 要转换的字符串。可以是 <see cref="DisplayAttribute"/> 的 Name、Description 或枚举字段的名称。
    /// </param>
    /// <returns>
    /// 返回对应的枚举值，如果没有找到匹配的字符串，则抛出异常。
    /// </returns>
    /// <exception cref="ArgumentException">
    /// 当指定的字符串无法匹配任何 <see cref="DisplayAttribute"/> 的 Name、Description 或枚举字段名称时抛出异常。
    /// </exception>
    public static TEnum FromDisplayString<TEnum>(this string displayString) where TEnum:struct
    {
        Type enumType = typeof(TEnum);

        // 检查并生成缓存
        if (!DisplayNameMap.ContainsKey(enumType))
        {
            var nameMap = new Dictionary<string, object?>();
            var descriptionMap = new Dictionary<string, object?>();

            foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (System.Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute)
                {
                    if (!string.IsNullOrEmpty(attribute.Name))
                    {
                        nameMap[attribute.Name] = field.GetValue(null);
                    }
                    if (!string.IsNullOrEmpty(attribute.Description))
                    {
                        descriptionMap[attribute.Description] = field.GetValue(null);
                    }
                }
                nameMap[field.Name] = field.GetValue(null);
            }

            DisplayNameMap[enumType] = nameMap;
            DisplayDescriptionMap[enumType] = descriptionMap;
        }

        // 从缓存中查找
        if (DisplayNameMap[enumType].TryGetValue(displayString, out var value) ||
            DisplayDescriptionMap[enumType].TryGetValue(displayString, out value))
        {
            return (TEnum)(value ?? throw new Exception("枚举值为空"));
        }

        throw new ArgumentException($"没有找到匹配的枚举值对于字符串 '{displayString}' 在枚举类型 {enumType.Name} 中。");
    }

    #endregion

    #region Try Catch

    /// <summary>
    /// Try Catch 简易封装 异步
    /// </summary>
    /// <param name="task"></param>
    /// <param name="onException"></param>
    /// <param name="onFinally"></param>
    /// <typeparam name="TException"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    public static async Task<TReturn?> TryAsync<TReturn, TException>(
        this Task<TReturn?> task,
        Action<TException>? onException = null,
        Action? onFinally = null
    )
        where TException:Exception
    {
        try
        {
            return await task;
        }
        catch (TException ex)
        {
            onException?.Invoke(ex);
            return default;
        }
        finally
        {
            onFinally?.Invoke();
        }
    }

    /// <summary>
    /// 无需指定异常类型的Try Catch 简易封装 异步
    /// </summary>
    /// <param name="task"></param>
    /// <param name="onException"></param>
    /// <param name="onFinally"></param>
    /// <typeparam name="TReturn"></typeparam>
    /// <returns></returns>
    public static async Task<TReturn?> TryAsync<TReturn>(
        this Task<TReturn?> task,
        Action<Exception>? onException = null,
        Action? onFinally = null
    )
    {
        try
        {
            return await task;
        }
        catch (Exception)
        {
            onException?.Invoke(new Exception());
            return default;
        }
        finally
        {
            onFinally?.Invoke();
        }
    }

    #endregion

    #region 查询相关

    /// <summary>
    /// 将查询结果转换为分页列表。
    /// </summary>
    /// <typeparam name="T">查询结果的类型。</typeparam>
    /// <param name="query">要分页的查询。</param>
    /// <param name="pageIndex">页码，从1开始。</param>
    /// <param name="pageSize">每页的记录数。</param>
    /// <returns>分页后的查询结果。</returns>
    public static IQueryable<T> ToPagedListAsync<T>(
        this IQueryable<T> query,
        int pageIndex,
        int pageSize)
    {
        if (pageIndex <= 0 || pageSize <= 0)
        {
            return query;
        }

        return query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);
    }

    #endregion
}
