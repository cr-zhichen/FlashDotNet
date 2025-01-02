using EasyCaching.Core;
using FlashDotNet.Infrastructure;
using Newtonsoft.Json;

namespace FlashDotNet.Services.CacheService;

/// <summary>
/// 基于 EasyCaching 的内存缓存服务
/// </summary>
[AddScopedAsImplementedInterfaces]
public class CacheService : ICacheService
{
    private readonly IEasyCachingProvider _cachingProvider;

    /// <summary>
    /// 初始化内存缓存服务实例
    /// </summary>
    public CacheService(IEasyCachingProvider cachingProvider)
    {
        _cachingProvider = cachingProvider;
    }

    /// <inheritdoc />
    public T? Get<T>(string key)
    {
        var cachedData = _cachingProvider.Get<string>(key).Value;
        if (cachedData == null)
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(cachedData);
    }

    /// <inheritdoc />
    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var serializedData = JsonConvert.SerializeObject(value);
        _cachingProvider.Set(key, serializedData, expiration ?? TimeSpan.FromMinutes(60));
    }

    /// <inheritdoc />
    public void Remove(string key)
    {
        _cachingProvider.Remove(key);
    }

    /// <inheritdoc />
    public bool Exists(string key)
    {
        return _cachingProvider.Exists(key);
    }
}
