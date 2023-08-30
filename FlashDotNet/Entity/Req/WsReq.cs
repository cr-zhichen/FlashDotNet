using Newtonsoft.Json.Linq;

namespace FlashDotNet.Entity.Req;

/// <summary>
/// 请求
/// </summary>
public class WsReq
{
    /// <summary>
    /// websocket请求的路由
    /// </summary>
    public string? Route { get; set; }

    /// <summary>
    /// 请求数据
    /// </summary>
    public JObject? Data { get; set; }
}