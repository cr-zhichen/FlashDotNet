using System.Diagnostics;
using FlashDotNet.Utils;

namespace FlashDotNet.Middleware;

/// <summary>
/// VueCli中间件
/// </summary>
public class VueCliMiddleware
{
    private static int _nodeServiceStarted = 0;
    private readonly RequestDelegate _next;
    private readonly int _nodeDevPorts;
    private readonly string _baseDirectory;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="next"></param>
    /// <param name="env"></param>
    /// <param name="nodeDevPorts"></param>
    public VueCliMiddleware(RequestDelegate next, IWebHostEnvironment env, int nodeDevPorts)
    {
        _next = next;
        _nodeDevPorts = nodeDevPorts;
        _baseDirectory = env.ContentRootPath;
    }

    /// <summary>
    /// 调用
    /// </summary>
    /// <param name="context"></param>
    public async Task InvokeAsync(HttpContext context)
    {
        if (Interlocked.CompareExchange(ref _nodeServiceStarted, 1, 0) == 0)
        {
            StartNodeService();
        }

        await _next(context);
    }

    /// <summary>
    /// 启动Node服务
    /// </summary>
    private void StartNodeService()
    {
        string fileName, arguments;
        if (OperatingSystem.IsWindows())
        {
            fileName = "cmd.exe";
            arguments = $"/c npm run dev -- --port {_nodeDevPorts}";
        }
        else
        {
            fileName = "/bin/bash";
            arguments = $"-c \"npm run dev -- --port {_nodeDevPorts}\"";
        }

        var vueCli = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = Path.Combine(_baseDirectory, "ClientApp"),
            UseShellExecute = false,
            RedirectStandardError = true
        };
        Process.Start(vueCli);
    }
}

/// <summary>
/// VueCli中间件扩展
/// </summary>
public static class VueCliMiddlewareExtensions
{
    /// <summary>
    /// 使用VueCli中间件
    /// </summary>
    /// <param name="app"></param>
    /// <param name="nodeDevPorts"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseVueCli(this IApplicationBuilder app, int? nodeDevPorts)
    {
        nodeDevPorts ??= PortSelection.GetAvailablePort();
        app.UseMiddleware<VueCliMiddleware>(nodeDevPorts);
        app.UseSpa(spa => { spa.UseProxyToSpaDevelopmentServer($"http://localhost:{nodeDevPorts}"); });
        return app;
    }
}