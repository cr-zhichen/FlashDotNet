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
            .Where(method => !method.IsVirtual || !IsDependencyInjected(method.DeclaringType!))
            .ToList();

        if (invalidMethods.Any())
        {
            foreach (var method in invalidMethods)
            {
                if (!method.IsVirtual)
                {
                    Console.WriteLine($"错误: 方法 '{method.Name}' 被 AbstractInterceptorAttribute 修饰，但不是虚方法。");
                }
                else if (!IsDependencyInjected(method.DeclaringType!))
                {
                    Console.WriteLine($"错误: 方法 '{method.Name}' 被 AbstractInterceptorAttribute 修饰，但其类未被依赖注入。");
                }
            }
            throw new InvalidOperationException($"共有 {invalidMethods.Count} 个方法不符合规则，请检查日志。");
        }
        return Task.CompletedTask;
    }

    private bool IsDependencyInjected(Type type)
    {
        // 检查类是否有依赖注入特性
        return type.GetCustomAttributes(typeof(AddTransientAttribute), false).Length > 0 ||
               type.GetCustomAttributes(typeof(AddTransientAsImplementedInterfacesAttribute), false).Length > 0;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
