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
        var now = DateTime.Now;
        TokenList.TokenLists.RemoveAll(token => token.ExpireTime < now);
        Console.WriteLine($"Token清理服务执行，清理后Token数量：{TokenList.TokenLists.Count}");
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