using FlashDotNet.Static;

namespace FlashDotNet.Utils;

/// <summary>
/// Token清理服务
/// </summary>
public class TokenCleanupService : IHostedService, IDisposable
{
    private Timer? _timer;

    /// <summary>
    /// 启动异步操作。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // 设置定时器，每小时执行一次
        _timer = new Timer(DoWork!, null, TimeSpan.Zero,
            TimeSpan.FromHours(1));

        return Task.CompletedTask;
    }

    /// <summary>
    /// 定时执行的方法
    /// </summary>
    /// <param name="state"></param>
    private void DoWork(object state)
    {
        int count = TokenWhiteList.RemoveExpiredToken();
        Console.WriteLine($"本次清理Token数量：{count}");
        Console.WriteLine($"剩余有效Token数量：{TokenWhiteList.GetTokenCount()}");
    }

    /// <summary>
    /// 停止异步操作。
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        _timer?.Dispose();
    }
}