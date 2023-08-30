namespace FlashDotNet.Static;

public static class TokenList
{
    public static List<TokenItem> TokenLists = new();

    /// <summary>
    /// Token暂存
    /// </summary>
    public class TokenItem
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}