using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using FlashDotNet.Data;
using FlashDotNet.DTOs;
using FlashDotNet.DTOs.WebSocket.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Route = FlashDotNet.Enum.Route;

namespace FlashDotNet.WS
{
    /// <summary>
    /// WebSocket控制器
    /// </summary>
    public class WebSocketController
    {
        /// <summary>
        /// 用户连接集合
        /// </summary>
        private static readonly ConcurrentDictionary<string, UserConnection> Connections =
            new ConcurrentDictionary<string, UserConnection>();

        /// <summary>
        /// 配置文件
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configuration"></param>
        public WebSocketController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// 处理WebSocket请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="webSocket"></param>
        public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            var socketId = context.Connection.Id; // 使用连接ID作为WebSocket的唯一标识
            Connections.TryAdd(socketId, new UserConnection(webSocket, socketId));

            var buffer = new byte[4096000];
            WebSocketReceiveResult result;

            IWsRe<JObject> send = new WsOk<JObject>
            {
                Route = Route.First.ToString(),
                Data = JObject.FromObject(new
                {
                    SocketId = socketId
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
                    //计算消息大小 单位：KB
                    double size = Math.Round((double)result.Count / 1024, 2);
                    Console.WriteLine(
                        $"接收到消息，大小：{size}KB，时间：{DateTime.Now:g}，连接ID：{socketId}，\n消息内容：\n{Encoding.UTF8.GetString(buffer, 0, result.Count)}\n");

                    // 接收到的消息
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    IWsRe<JObject> re;

                    try
                    {
                        WsReq req = JsonConvert.DeserializeObject<WsReq>(receivedMessage) ??
                                    throw new InvalidOperationException();
                        await WebsocketProcess.Process(Connections[socketId], req);
                    }
                    catch (Exception e)
                    {
                        re = new WsError<JObject>
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
}