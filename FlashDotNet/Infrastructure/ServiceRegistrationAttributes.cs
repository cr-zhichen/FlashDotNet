// ReSharper disable ClassNeverInstantiated.Global

namespace FlashDotNet.Infrastructure;

/// <summary>
/// 标记特性
/// 用于自动注册AddScoped。
/// AddScoped用于在依赖注入容器中注册服务，其生命周期为Scoped，意味着在每个请求范围内服务实例是唯一的。
/// </summary>
public class MarkerAddScoped : System.Attribute;

/// <summary>
/// 标记特性
/// 用于自动注册AddScoped,并使用接口注册。
/// AddScoped用于在依赖注入容器中注册服务，其生命周期为Scoped，意味着在每个请求范围内服务实例是唯一的。此特性还表示服务通过实现的接口进行注册，便于解耦和灵活替换。
/// </summary>
public class MarkerAddScopedAsImplementedInterfaces : System.Attribute;

/// <summary>
/// 标记特性
/// 用于自动注册AddTransient。
/// AddTransient用于在依赖注入容器中注册服务，其生命周期为Transient，意味着每次请求服务时都会创建一个新的实例。
/// </summary>
public class MarkerAddTransient : System.Attribute;

/// <summary>
/// 标记特性
/// 用于自动注册AddTransient,并使用接口注册。
/// AddTransient用于在依赖注入容器中注册服务，其生命周期为Transient，意味着每次请求服务时都会创建一个新的实例。此特性还表示服务通过实现的接口进行注册，增加了代码的抽象层次和可测试性。
/// </summary>
public class MarkerAddTransientAsImplementedInterfaces : System.Attribute;

/// <summary>
/// 标记特性
/// 用于自动注册AddSingleton。
/// AddSingleton用于在依赖注入容器中注册服务，其生命周期为Singleton，意味着整个应用生命周期内只创建服务的一个实例。
/// </summary>
public class MarkerAddSingleton : System.Attribute;

/// <summary>
/// 标记特性
/// 用于自动注册AddSingleton,并使用接口注册。
/// AddSingleton用于在依赖注入容器中注册服务，其生命周期为Singleton，意味着整个应用生命周期内只创建服务的一个实例。通过接口注册服务提供了更高的灵活性和解耦能力。
/// </summary>
public class MarkerAddSingletonAsImplementedInterfaces : System.Attribute;

/// <summary>
/// 标记特性
/// 用于自动注册AddHostedService。
/// AddHostedService用于在依赖注入容器中注册后台服务，这些服务在应用启动时启动，并在应用停止时停止。用于实现长运行的后台任务。
/// </summary>
public class MarkerAddHostedService : System.Attribute;
