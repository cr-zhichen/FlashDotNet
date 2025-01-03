using FlashDotNet.Enum;
using FlashDotNet.Models;

namespace FlashDotNet.Repositories.UserRepository;

/// <summary>
/// 和User相关的数据库操作
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 判断数据库中用户表是否为空
    /// </summary>
    /// <returns></returns>
    Task<bool> IsEmptyAsync();

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="username"></param>
    /// <param name="argon2Password"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    Task<User> CreateUserAsync(string username, string argon2Password, UserRole role = UserRole.User);

    /// <summary>
    /// 判断密码是否正确
    /// </summary>
    /// <param name="username"></param>
    /// <param name="argon2Password"></param>
    /// <returns></returns>
    Task<bool> CheckPasswordAsync(string username, string argon2Password);

    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<User?> GetUserAsync(string username);

    /// <summary>
    /// 根据用户id获取用户
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<User?> GetUserAsync(Guid userId);

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns></returns>
    Task<(int total, List<User> list)> GetUserListAsync(int pageIndex, int pageSize);
}
