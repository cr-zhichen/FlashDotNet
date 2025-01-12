using FlashDotNet.Converters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FlashDotNet.Utils;

/// <summary>
/// Json配置帮助类
/// </summary>
public static class JsonConfigurationHelper
{
    /// <summary>
    /// 获取默认的Json配置
    /// </summary>
    /// <returns></returns>
    public static JsonSerializerSettings GetDefaultSettings()
    {
        return new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                // 注册通用的枚举转换器
                new JsonStringEnumDisplayConverter()
            },
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };
    }

    /// <summary>
    /// 应用默认的Json配置
    /// </summary>
    /// <param name="options"></param>
    public static void ApplyDefaultSettings(this MvcNewtonsoftJsonOptions options)
    {
        var settings = GetDefaultSettings();
        options.SerializerSettings.Converters = settings.Converters;
        options.SerializerSettings.ReferenceLoopHandling = settings.ReferenceLoopHandling;
        options.SerializerSettings.NullValueHandling = settings.NullValueHandling;
        options.SerializerSettings.ContractResolver = settings.ContractResolver;
    }

    /// <summary>
    /// 应用默认的Json配置
    /// </summary>
    /// <param name="options"></param>
    public static void ApplyDefaultSettings(this NewtonsoftJsonHubProtocolOptions options)
    {
        var settings = GetDefaultSettings();
        options.PayloadSerializerSettings.Converters = settings.Converters;
        options.PayloadSerializerSettings.ReferenceLoopHandling = settings.ReferenceLoopHandling;
        options.PayloadSerializerSettings.NullValueHandling = settings.NullValueHandling;
        options.PayloadSerializerSettings.ContractResolver = settings.ContractResolver;
    }
}
