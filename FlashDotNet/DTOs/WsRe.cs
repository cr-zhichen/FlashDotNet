﻿using FlashDotNet.Utils;
using Newtonsoft.Json;

namespace FlashDotNet.DTOs;

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
    public string Route { get; set; }

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
    public string Route { get; set; } = null!;

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
/// 失败响应
/// </summary>
/// <typeparam name="T"></typeparam>
public class WsError<T> : IWsRe<T>
{
    /// <summary>
    /// websocket返回的路由
    /// </summary>
    [JsonProperty("route")]
    public string Route { get; set; } = Enum.Route.Error.GetDisplayName();

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
