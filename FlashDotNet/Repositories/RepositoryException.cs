namespace FlashDotNet.Repositories;

/// <summary>
/// 表示存储库操作期间发生的异常。
/// </summary>
public class RepositoryException : Exception
{
    /// <summary>
    /// 使用指定的错误消息初始化 <see cref="RepositoryException"/> 类的新实例。
    /// </summary>
    /// <param name="message">描述错误的消息。</param>
    public RepositoryException(string message) : base(message)
    {
    }

    /// <summary>
    /// 使用指定的错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="RepositoryException"/> 类的新实例。
    /// </summary>
    /// <param name="message">描述错误的消息。</param>
    /// <param name="innerException">导致当前异常的异常。</param>
    public RepositoryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
