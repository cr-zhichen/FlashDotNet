using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using FlashDotNet.Data;
using FlashDotNet.DTOs;
using FlashDotNet.DTOs.WebSocket.Requests;
using FlashDotNet.Infrastructure;
using FlashDotNet.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Route = FlashDotNet.Enum.Route;

namespace FlashDotNet.WS;

/// <summary>
/// WebSocket控制器
/// </summary>
[AddScoped]
public class WebSocketController
{
    /// <summary>
    /// 用户连接集合
    /// </summary>
    private readonly static ConcurrentDictionary<string, UserConnection> Connections =
        new ConcurrentDictionary<string, UserConnection>();

    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;
    private readonly ILogger<WebSocketController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public WebSocketController(AppDbContext context, IConfiguration configuration, ILogger<WebSocketController> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 处理WebSocket请求
    /// </summary>
    /// <param name="context"></param>
    /// <param name="webSocket"></param>
    public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
    {
        var socketId = context.Connection.Id; // 使用连接ID作为WebSocket的唯一标识
        Connections.TryAdd(socketId, new UserConnection(webSocket));

        var initialBufferSize = 4096; // 初始缓冲区大小
        var buffer = new byte[initialBufferSize];
        WebSocketReceiveResult result;

        IWsRe<JObject> send = new WsOk<JObject>
        {
            Route = Route.First.GetDisplayName(),
            Data = JObject.FromObject(new
            {
                socketId = socketId
            }),
        };

        // 发送首次连接的消息
        var sendBuffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(send));
        await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer, 0, sendBuffer.Length),
            WebSocketMessageType.Text, true, CancellationToken.None);

        do
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType != WebSocketMessageType.Close)
            {
                // 如果消息大小超过当前缓冲区大小，则动态调整缓冲区大小
                if (result.Count > buffer.Length)
                {
                    buffer = new byte[result.Count];
                }

                //计算消息大小 单位：KB
                double size = Math.Round((double)result.Count / 1024, 2);
                _logger.LogInformation(
                    $"接收到消息，大小：{size}KB，时间：{DateTime.Now:g}，连接ID：{socketId}，\n消息内容：\n{Encoding.UTF8.GetString(buffer, 0, result.Count)}\n");

                // 接收到的消息
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                try
                {
                    WsReq req = JsonConvert.DeserializeObject<WsReq>(receivedMessage) ??
                                throw new InvalidOperationException();
                    await WebsocketProcess.Process(Connections[socketId], req);
                }
                catch (Exception e)
                {
                    IWsRe<JObject> re = new WsError<JObject>
                    {
                        Message = "请求数据错误，请检查数据是否正确",
                        Data = JObject.FromObject(e),
                    };
                    // 生成返回的消息
                    var responseBuffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(re));
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer, 0, responseBuffer.Length),
                        result.MessageType, result.EndOfMessage, CancellationToken.None);
                }
            }
        } while (!result.CloseStatus.HasValue);

        Connections.TryRemove(socketId, out _);
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }

    /// <summary>
    /// 广播消息
    /// </summary>
    /// <param name="message"></param>
    public static async Task BroadcastMessageAsync(string message)
    {
        foreach (var pair in Connections)
        {
            if (pair.Value.WebSocket.State == WebSocketState.Open)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await pair.Value.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length),
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
