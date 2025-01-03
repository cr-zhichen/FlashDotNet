using FlashDotNet.Enum;
using Newtonsoft.Json;

namespace FlashDotNet.DTOs.WebSocket;

/// <summary>
/// 响应
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IWsRe<T>
{
    /// <summary>
    /// websocket返回的路由
    /// </summary>
    [JsonProperty("route")]
    public WsRoute Route { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }

    /// <summary>
    /// 返回数据
    /// </summary>
    [JsonProperty("data")]
    public T? Data { get; set; }
}

/// <summary>
/// 成功响应
/// </summary>
/// <typeparam name="T"></typeparam>
public class WsOk<T> : IWsRe<T>
{
    /// <summary>
    /// websocket返回的路由
    /// </summary>
    [JsonProperty("route")]
    public required WsRoute Route { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    [JsonProperty("message")]
    public required string? Message { get; set; }

    /// <summary>
    /// 返回数据
    /// </summary>
    [JsonProperty("data")]
    public required T? Data { get; set; }
}

/// <summary>
/// 失败响应
/// </summary>
/// <typeparam name="T"></typeparam>
public class WsError<T> : IWsRe<T>
{
    /// <summary>
    /// websocket返回的路由
    /// </summary>
    [JsonProperty("route")]
    public WsRoute Route { get; set; } = WsRoute.Error;

    /// <summary>
    /// 消息
    /// </summary>
    [JsonProperty("message")]
    public required string? Message { get; set; }

    /// <summary>
    /// 返回数据
    /// </summary>
    [JsonProperty("data")]
    public required T? Data { get; set; }
}
