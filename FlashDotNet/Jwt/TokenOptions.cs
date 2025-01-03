using System.Text.Json.Serialization;

namespace FlashDotNet.Jwt;

/// <summary>
/// Token相关的配置
/// </summary>
public class TokenOptions
{
    /// <summary>
    /// 秘钥
    /// </summary>
    public string SecretKey { get; set; } = null!;

    /// <summary>
    /// 签发者
    /// </summary>
    public string Issuer { get; set; } = null!;

    /// <summary>
    /// 接收者
    /// </summary>
    public string Audience { get; set; } = null!;

    /// <summary>
    /// 过期时间（分钟）
    /// </summary>
    public int ExpireMinutes { get; set; } = 30;
}

/// <summary>
/// 令牌数据中的用户信息
/// </summary>
public class UserInfoTokenData
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [JsonPropertyName("user_id")]
    public required string UserId { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    [JsonPropertyName("role")]
    public required string Role { get; set; }

    /// <summary>
    /// Token版本号
    /// </summary>
    [JsonPropertyName("version")]
    public required string Version { get; set; }
}
