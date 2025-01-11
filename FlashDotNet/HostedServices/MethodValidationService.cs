using System.Reflection;
using AspectCore.DynamicProxy;
using FlashDotNet.Infrastructure;

namespace FlashDotNet.HostedServices;

/// <summary>
/// 方法验证服务
/// </summary>
[AddHostedService]
public class MethodValidationService : IHostedService
{
    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // 使用 LINQ 查找所有不符合规则的方法
        var invalidMethods = assembly.GetTypes()
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(method => method.GetCustomAttributes(typeof(AbstractInterceptorAttribute), false).Length > 0)
            .Where(method => !method.IsVirtual)
            .ToList();

        if (invalidMethods.Any())
        {
            var errorMessages = new List<string>();
            foreach (var method in invalidMethods)
            {
                errorMessages.Add($"错误: 方法 '{method.Name}' 被 AbstractInterceptorAttribute 修饰，但不是虚方法。");
            }
            Console.WriteLine(string.Join(Environment.NewLine, errorMessages));
            throw new InvalidOperationException($"共有 {invalidMethods.Count} 个方法不符合规则，请检查日志。");
        }
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
