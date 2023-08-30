namespace FlashDotNet.Jwt;

public class TokenOptions
{
    /// <summary>
    /// 秘钥
    /// </summary>
    public string SecretKey { get; set; }

    /// <summary>
    /// 签发者
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// 接收者
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// 过期时间（分钟）
    /// </summary>
    public int ExpireMinutes { get; set; } = 30;
}