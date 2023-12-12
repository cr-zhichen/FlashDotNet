using System.Collections.Concurrent;

namespace FlashDotNet.Jwt;

/// <summary>
/// Token白名单列表
/// </summary>
public static class TokenWhiteList
{
    /// <summary>
    /// Token白名单字典
    /// Key - Token
    /// Value - 用户ID,过期时间
    /// </summary>
    private static readonly ConcurrentDictionary<string, (string, DateTime)> TokenDictionary = new();

    /// <summary>
    /// 添加Token
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <param name="expireTime">过期时间</param>
    public static void AddToken(string userId, string token, DateTime expireTime)
    {
        TokenDictionary.TryAdd(token, (userId, expireTime));
    }

    /// <summary>
    /// 判断Token是否存在
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static bool ContainsToken(string token)
    {
        return TokenDictionary.ContainsKey(token);
    }

    /// <summary>
    /// 移除Token
    /// </summary>
    /// <param name="token"></param>
    public static void RemoveToken(string token)
    {
        TokenDictionary.TryRemove(token, out _);
    }

    /// <summary>
    /// 根据用户ID移除Token
    /// </summary>
    /// <param name="userId"></param>
    public static void RemoveTokenByUserId(string userId)
    {
        foreach (var (key, value) in TokenDictionary)
        {
            if (value.Item1 == userId)
            {
                TokenDictionary.TryRemove(key, out _);
            }
        }
    }

    /// <summary>
    /// 删除过期的Token
    /// </summary>
    public static int RemoveExpiredToken()
    {
        int count = 0;
        foreach (var (key, value) in TokenDictionary)
        {
            if (value.Item2 < DateTime.Now)
            {
                count++;
                TokenDictionary.TryRemove(key, out _);
            }
        }

        return count;
    }

    /// <summary>
    /// 获取Token数量
    /// </summary>
    /// <returns></returns>
    public static int GetTokenCount()
    {
        return TokenDictionary.Count;
    }
}