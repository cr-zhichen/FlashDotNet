namespace FlashDotNet.Infrastructure;

/// <summary>
/// 表示标记接口。
/// 用于自动注册AddScoped
/// </summary>
public interface IMarkerAddScoped
{
}

/// <summary>
/// 表示标记接口。
/// 用于自动注册AddScoped,并使用接口注册
/// </summary>
public interface IMarkerAddScopedAsImplementedInterfaces
{
}

/// <summary>
/// 表示标记接口。
/// 用于自动注册AddTransient
/// </summary>
public interface IMarkerAddTransient
{
}

/// <summary>
/// 表示标记接口。
/// 用于自动注册AddTransient,并使用接口注册
/// </summary>
public interface IMarkerAddTransientAsImplementedInterfaces
{
}

/// <summary>
/// 表示标记接口。
/// 用于自动注册AddSingleton
/// </summary>
public interface IMarkerAddSingleton
{
}

/// <summary>
/// 表示标记接口。
/// 用于自动注册AddSingleton,并使用接口注册
/// </summary>
public interface IMarkerAddSingletonAsImplementedInterfaces
{
}

/// <summary>
/// 表示标记接口。
/// 用于自动注册AddHostedService
/// </summary>
public interface IMarkerAddHostedService
{
}