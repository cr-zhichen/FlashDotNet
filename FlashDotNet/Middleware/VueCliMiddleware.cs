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
        string clientAppDirectory = Path.Combine(_baseDirectory, "ClientApp");

        // 检查 node_modules 目录是否存在
        if (!Directory.Exists(Path.Combine(clientAppDirectory, "node_modules")))
        {
            Console.WriteLine("node_modules目录不存在，开始安装npm包。");
            // 如果不存在，先运行 npm install 并等待其完成
            if (!RunCommandAndWait("npm install", clientAppDirectory))
            {
                Console.WriteLine("npm安装失败。请检查错误，然后重试。");
                return;
            }
        }

        // npm install 完成后，运行 npm run dev
        RunCommand("npm run dev -- --port " + _nodeDevPorts, clientAppDirectory);
    }

    /// <summary>
    /// 运行npm install命令并等待其完成
    /// </summary>
    /// <param name="command"></param>
    /// <param name="workingDirectory"></param>
    /// <returns></returns>
    private bool RunCommandAndWait(string command, string workingDirectory)
    {
        string fileName, arguments;
        if (OperatingSystem.IsWindows())
        {
            fileName = "cmd.exe";
            arguments = $"/c {command}";
        }
        else
        {
            fileName = "/bin/bash";
            arguments = $"-c \"{command}\"";
        }

        var processStartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = false,
            RedirectStandardError = false
        };

        using (var process = Process.Start(processStartInfo))
        {
            process.WaitForExit(); // 等待进程完成
            return process.ExitCode == 0; // 返回进程是否成功完成
        }
    }

    /// <summary>
    /// 运行npm run dev命令
    /// </summary>
    /// <param name="command"></param>
    /// <param name="workingDirectory"></param>
    private void RunCommand(string command, string workingDirectory)
    {
        string fileName, arguments;
        if (OperatingSystem.IsWindows())
        {
            fileName = "cmd.exe";
            arguments = $"/c {command}";
        }
        else
        {
            fileName = "/bin/bash";
            arguments = $"-c \"{command}\"";
        }

        var processStartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardError = true
        };

        Process.Start(processStartInfo);
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