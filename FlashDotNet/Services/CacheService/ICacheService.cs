namespace FlashDotNet.Services.CacheService;

/// <summary>
/// 缓存服务接口
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// 获取缓存项
    /// </summary>
    /// <typeparam name="T">缓存项的类型</typeparam>
    /// <param name="key">缓存项的键</param>
    /// <returns>缓存项的值</returns>
    T? Get<T>(string key);

    /// <summary>
    /// 设置缓存项
    /// </summary>
    /// <typeparam name="T">缓存项的类型</typeparam>
    /// <param name="key">缓存项的键</param>
    /// <param name="value">缓存项的值</param>
    /// <param name="expiration">缓存项的过期时间</param>
    void Set<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// 移除缓存项
    /// </summary>
    /// <param name="key">缓存项的键</param>
    void Remove(string key);

    /// <summary>
    /// 判断缓存项是否存在
    /// </summary>
    /// <param name="key">缓存项的键</param>
    /// <returns>缓存项是否存在</returns>
    bool Exists(string key);
}
