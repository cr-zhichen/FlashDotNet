// ReSharper disable ClassNeverInstantiated.Global

namespace FlashDotNet.Infrastructure;

/// <summary>
/// 标记特性
/// 用于自动注册AddScoped。
/// AddScoped用于在依赖注入容器中注册服务，其生命周期为Scoped，意味着在每个请求范围内服务实例是唯一的。
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AddScopedAttribute : System.Attribute;

/// <summary>
/// 标记特性
/// 用于自动注册AddScoped,并使用接口注册。
/// AddScoped用于在依赖注入容器中注册服务，其生命周期为Scoped，意味着在每个请求范围内服务实例是唯一的。此特性还表示服务通过实现的接口进行注册，便于解耦和灵活替换。
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AddScopedAsImplementedInterfacesAttribute : System.Attribute;

/// <summary>
/// 标记特性
/// 用于自动注册AddTransient。
/// AddTransient用于在依赖注入容器中注册服务，其生命周期为Transient，意味着每次请求服务时都会创建一个新的实例。
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AddTransientAttribute : System.Attribute;

/// <summary>
/// 标记特性
/// 用于自动注册AddTransient,并使用接口注册。
/// AddTransient用于在依赖注入容器中注册服务，其生命周期为Transient，意味着每次请求服务时都会创建一个新的实例。此特性还表示服务通过实现的接口进行注册，增加了代码的抽象层次和可测试性。
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AddTransientAsImplementedInterfacesAttribute : System.Attribute;

/// <summary>
/// 标记特性
/// 用于自动注册AddSingleton。
/// AddSingleton用于在依赖注入容器中注册服务，其生命周期为Singleton，意味着整个应用生命周期内只创建服务的一个实例。
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AddSingletonAttribute : System.Attribute;

/// <summary>
/// 标记特性
/// 用于自动注册AddSingleton,并使用接口注册。
/// AddSingleton用于在依赖注入容器中注册服务，其生命周期为Singleton，意味着整个应用生命周期内只创建服务的一个实例。通过接口注册服务提供了更高的灵活性和解耦能力。
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AddSingletonAsImplementedInterfacesAttribute : System.Attribute;

/// <summary>
/// 标记特性
/// 用于自动注册AddHostedService。
/// AddHostedService用于在依赖注入容器中注册后台服务，这些服务在应用启动时启动，并在应用停止时停止。用于实现长运行的后台任务。
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AddHostedServiceAttribute : System.Attribute;
