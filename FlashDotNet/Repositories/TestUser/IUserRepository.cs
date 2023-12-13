using FlashDotNet.Enum;
using FlashDotNet.Models;

namespace FlashDotNet.Repositories.TestUser;

/// <summary>
/// 和TestUser相关的数据库操作
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
    Task<UserInfo> CreateUserAsync(string username, string argon2Password, UserRole role = UserRole.User);

    /// <summary>
    /// 判断密码是否正确
    /// </summary>
    /// <param name="username"></param>
    /// <param name="argon2Password"></param>
    /// <returns></returns>
    Task<bool> CheckPasswordAsync(string username, string argon2Password);

    /// <summary>
    /// 获取用户角色
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<UserRole> GetUserRoleAsync(string username);

    /// <summary>
    /// 获取用户角色
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<UserRole> GetUserRoleAsync(int userId);

    /// <summary>
    /// 根据用户名获取用户id
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<int> GetUserIdAsync(string username);

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <returns></returns>
    Task<List<UserInfo>> GetUserListAsync();
}