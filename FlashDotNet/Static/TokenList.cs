namespace FlashDotNet.Static;

/// <summary>
/// Token白名单列表
/// </summary>
public static class TokenList
{
    /// <summary>
    /// Token白名单列表
    /// </summary>
    public static readonly List<TokenItem> TokenLists = new();

    /// <summary>
    /// Token暂存
    /// </summary>
    public class TokenItem
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }
    }
}