using System.Net;
using System.Net.Sockets;

namespace FlashDotNet.Utils;

/// <summary>
/// 端口选择
/// </summary>
public static class PortSelection
{
    /// <summary>
    /// 获取可用端口
    /// </summary>
    /// <param name="startPort"></param>
    /// <param name="endPort"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static int GetAvailablePort(int startPort = 1024, int endPort = 65535)
    {
        var random = new Random();
        var portRange = Enumerable.Range(startPort, endPort - startPort).OrderBy(x => random.Next()).ToList();

        foreach (var port in portRange)
        {
            if (IsPortAvailable(port))
                return port;
        }

        throw new Exception("No available ports found.");
    }

    /// <summary>
    /// 判断端口是否可用
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    private static bool IsPortAvailable(int port)
    {
        TcpListener? client = null;
        try
        {
            client = new TcpListener(IPAddress.Loopback, port);
            client.Start();
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            client?.Stop();
        }
    }
}