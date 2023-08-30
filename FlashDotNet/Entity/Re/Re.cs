using FlashDotNet.Entity.Enum;

namespace FlashDotNet.Entity.Re;

/// <summary>
/// 响应
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRe<T>
{
    /// <summary>
    /// 状态码
    /// </summary>
    public Code Code { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 返回数据
    /// </summary>
    public T? Data { get; set; }
}

/// <summary>
/// 成功响应
/// </summary>
/// <typeparam name="T"></typeparam>
public class Ok<T> : IRe<T>
{
    /// <summary>
    /// 状态码，默认为成功
    /// </summary>
    public Code Code { get; set; } = Code.Success;

    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 返回数据
    /// </summary>
    public T? Data { get; set; }
}

/// <summary>
/// 失败响应
/// </summary>
/// <typeparam name="T"></typeparam>
public class Error<T> : IRe<T>
{
    /// <summary>
    /// 状态码，默认为未知错误
    /// </summary>
    public Code Code { get; set; } = Code.Error;

    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 返回数据
    /// </summary>
    public T? Data { get; set; }
}

/// <summary>
/// 响应
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IWsRe<T>
{
    /// <summary>
    /// websocket返回的路由
    /// </summary>
    public string Route { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 返回数据
    /// </summary>
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
    public string Route { get; set; } = null!;

    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 返回数据
    /// </summary>
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
    public string Route { get; set; } = Enum.Route.Error.ToString();

    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 返回数据
    /// </summary>
    public T? Data { get; set; }
}