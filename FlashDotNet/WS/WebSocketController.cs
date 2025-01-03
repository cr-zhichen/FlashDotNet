using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using FlashDotNet.DTOs;
using FlashDotNet.DTOs.WebSocket;
using FlashDotNet.Enum;
using FlashDotNet.Infrastructure;
using FlashDotNet.Utils;
using Newtonsoft.Json;

namespace FlashDotNet.WS;

/// <summary>
/// WebSocket控制器
/// </summary>
[AddScoped]
public class WebSocketController
{
    /// <summary>
    /// 用户连接集合（线程安全）
    /// </summary>
    private readonly static ConcurrentDictionary<string, UserConnection> Connections
        = new ConcurrentDictionary<string, UserConnection>();

    private readonly WebsocketProcess _websocketProcess;
    private readonly ILogger<WebSocketController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public WebSocketController(ILogger<WebSocketController> logger, WebsocketProcess websocketProcess)
    {
        _logger = logger;
        _websocketProcess = websocketProcess;
    }

    /// <summary>
    /// 处理WebSocket连接
    /// </summary>
    public async Task HandleWebSocketAsync(HttpContext httpContext, WebSocket webSocket)
    {
        var socketId = httpContext.Connection.Id; // 使用连接ID作为WebSocket的唯一标识
        Connections.TryAdd(socketId, new UserConnection(webSocket, socketId));

        try
        {
            // 发送连接成功的初始消息
            await SendInitialMessageAsync(webSocket, socketId);

            // 循环接收消息，直到WebSocket关闭
            await ReceiveMessageLoopAsync(socketId, webSocket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"WebSocket异常，连接ID：{socketId}");
        }
        finally
        {
            // 清理并关闭连接
            await CloseWebSocketAsync(socketId);
        }
    }

    /// <summary>
    /// 循环接收消息并处理
    /// </summary>
    private async Task ReceiveMessageLoopAsync(string socketId, WebSocket webSocket)
    {
        var buffer = new byte[4096]; // 建议的初始缓冲区大小
        var messageBuffer = new List<byte>(); // 用于拼接分段消息

        while (webSocket.State == WebSocketState.Open)
        {

            try
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                // 拼接分段消息
                messageBuffer.AddRange(buffer[..result.Count]);

                // 如果消息还没结束，则继续接收
                if (!result.EndOfMessage) continue;

                // 消息完整，开始处理
                var message = Encoding.UTF8.GetString(messageBuffer.ToArray());
                messageBuffer.Clear(); // 重置

                _logger.LogInformation(
                    $"接收到消息，长度：{message.Length} 字符，连接ID：{socketId}，时间：{DateTime.Now:g}\n" +
                    $"消息内容：\n{message}");

                // 处理消息
                await ProcessReceivedMessageAsync(socketId, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"接收/处理消息时出现异常，连接ID：{socketId}, 当前消息大小：{messageBuffer.Count} 字节");

                // 如果出现异常可酌情选择：清空缓冲区并继续，或者直接跳出循环
                messageBuffer.Clear();
                break;
            }
        }
    }

    /// <summary>
    /// 处理接收到的消息
    /// </summary>
    private async Task ProcessReceivedMessageAsync(string socketId, string rawMessage)
    {
        if (!Connections.TryGetValue(socketId, out var userConnection)) return;

        var webSocket = userConnection.WebSocket;
        try
        {
            // 尝试反序列化请求
            WsReq? req = JsonConvert.DeserializeObject<WsReq>(rawMessage, JsonConfigurationHelper.GetDefaultSettings());

            if (req == null)
            {
                throw new InvalidOperationException("反序列化后的对象为null");
            }

            // 进行业务处理
            await _websocketProcess.Process(userConnection, req);
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx, $"JSON解析错误，连接ID：{socketId}");

            // 返回错误消息
            var errorResponse = new WsError<object>
            {
                Message = "Json解析错误，请检查数据是否正确",
                Data = "收到的消息：" + rawMessage
            };
            await SendAsync(webSocket, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"处理消息时出现异常，连接ID：{socketId}");

            // 返回错误消息
            var errorResponse = new WsError<object>
            {
                Message = "请求数据错误，请检查数据是否正确",
                Data = new { ex.Message, ex.StackTrace }
            };
            await SendAsync(webSocket, errorResponse);
        }
    }

    /// <summary>
    /// 发送初始消息，告知客户端连接ID等信息
    /// </summary>
    private async Task SendInitialMessageAsync(WebSocket webSocket, string socketId)
    {
        var initResponse = new WsOk<object>
        {
            Route = WsRoute.First,
            Message = "连接成功",
            Data = new
            {
                SocketId = socketId
            }
        };
        await SendAsync(webSocket, initResponse);
    }

    /// <summary>
    /// 广播消息
    /// </summary>
    public static async Task BroadcastMessageAsync<T>(IWsRe<T> data)
    {
        var tasks = Connections.Values
            .Where(conn => conn.WebSocket.State == WebSocketState.Open)
            .Select(conn => SendAsync(conn.WebSocket, data));

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 关闭WebSocket连接
    /// </summary>
    public static async Task CloseWebSocketAsync(string socketId)
    {
        if (Connections.TryRemove(socketId, out var connection))
        {
            if (connection.WebSocket.State == WebSocketState.Open)
            {
                await connection.WebSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Close WebSocket",
                    CancellationToken.None);
            }
        }
    }

    /// <summary>
    /// 简化的发送方法：可发送对象或字符串
    /// </summary>
    public static async Task SendAsync<T>(WebSocket webSocket, IWsRe<T> data)
    {
        if (webSocket.State != WebSocketState.Open) return;

        string message = JsonConvert.SerializeObject(data, JsonConfigurationHelper.GetDefaultSettings());

        var bytes = Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(bytes);

        await webSocket.SendAsync(
            segment,
            WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken: CancellationToken.None);
    }
}
