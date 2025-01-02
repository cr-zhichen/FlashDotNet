using FlashDotNet.Infrastructure;
using FlashDotNet.Services.CacheService;

namespace FlashDotNet.Services.Captcha;

/// <summary>
/// 用于生成和管理验证码令牌的服务接口。
/// </summary>
public interface ICaptchaTokenService
{
    /// <summary>
    /// 生成一个新的验证码令牌。
    /// </summary>
    /// <returns>一个唯一的验证码令牌字符串。</returns>
    string GenerateToken();

    /// <summary>
    /// 保存验证码数据到缓存中。
    /// </summary>
    /// <param name="token">验证码令牌。</param>
    /// <param name="key">验证码数据的键。</param>
    /// <param name="value">要存储的验证码值。</param>
    /// <param name="expiration">缓存数据的过期时间，默认为5分钟。</param>
    void SaveCaptchaData(string token, string key, object value, TimeSpan? expiration = null);

    /// <summary>
    /// 从缓存中获取验证码数据。
    /// </summary>
    /// <typeparam name="T">要返回的数据类型。</typeparam>
    /// <param name="token">验证码令牌。</param>
    /// <param name="key">验证码数据的键。</param>
    /// <returns>存储的验证码数据，如果未找到则为 null。</returns>
    T? GetCaptchaData<T>(string token, string key);

    /// <summary>
    /// 从缓存中删除指定的验证码数据。
    /// </summary>
    /// <param name="token">验证码令牌。</param>
    /// <param name="key">验证码数据的键。</param>
    void RemoveCaptchaData(string token, string key);

    /// <summary>
    /// 检查指定的验证码数据是否存在于缓存中。
    /// </summary>
    /// <param name="token">验证码令牌。</param>
    /// <param name="key">验证码数据的键。</param>
    /// <returns>如果数据存在则为 true，否则为 false。</returns>
    bool ExistsCaptchaData(string token, string key);
}

/// <summary>
/// 验证码令牌服务的实现类。
/// </summary>
[AddScopedAsImplementedInterfaces]
public class CaptchaTokenService : ICaptchaTokenService
{
    private readonly ICacheService _cacheService;
    private const string Prefix = "CAPTCHA:";

    /// <summary>
    /// 初始化 <see cref="CaptchaTokenService"/> 类的新实例。
    /// </summary>
    /// <param name="cacheService">缓存服务的实例。</param>
    public CaptchaTokenService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    /// <summary>
    /// 生成一个新的验证码令牌。
    /// </summary>
    /// <returns>生成的验证码令牌字符串。</returns>
    public string GenerateToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// 生成缓存键。
    /// </summary>
    /// <param name="token">验证码令牌。</param>
    /// <param name="key">验证码数据的键。</param>
    /// <returns>生成的缓存键字符串。</returns>
    private string GetCacheKey(string token, string key)
    {
        return $"{Prefix}{token}:{key}";
    }

    /// <summary>
    /// 将验证码数据保存到缓存中。
    /// </summary>
    /// <param name="token">验证码令牌。</param>
    /// <param name="key">验证码数据的键。</param>
    /// <param name="value">要存储的验证码值。</param>
    /// <param name="expiration">缓存数据的过期时间，默认为5分钟。</param>
    public void SaveCaptchaData(string token, string key, object value, TimeSpan? expiration = null)
    {
        expiration ??= TimeSpan.FromMinutes(5);

        string cacheKey = GetCacheKey(token, key);
        _cacheService.Set(cacheKey, value, expiration);
    }

    /// <summary>
    /// 从缓存中获取验证码数据。
    /// </summary>
    /// <typeparam name="T">要返回的数据类型。</typeparam>
    /// <param name="token">验证码令牌。</param>
    /// <param name="key">验证码数据的键。</param>
    /// <returns>存储的验证码数据，如果未找到则为 null。</returns>
    public T? GetCaptchaData<T>(string token, string key)
    {
        string cacheKey = GetCacheKey(token, key);
        return _cacheService.Get<T>(cacheKey);
    }

    /// <summary>
    /// 从缓存中删除指定的验证码数据。
    /// </summary>
    /// <param name="token">验证码令牌。</param>
    /// <param name="key">验证码数据的键。</param>
    public void RemoveCaptchaData(string token, string key)
    {
        string cacheKey = GetCacheKey(token, key);
        _cacheService.Remove(cacheKey);
    }

    /// <summary>
    /// 检查指定的验证码数据是否存在于缓存中。
    /// </summary>
    /// <param name="token">验证码令牌。</param>
    /// <param name="key">验证码数据的键。</param>
    /// <returns>如果数据存在则为 true，否则为 false。</returns>
    public bool ExistsCaptchaData(string token, string key)
    {
        string cacheKey = GetCacheKey(token, key);
        return _cacheService.Exists(cacheKey);
    }
}
