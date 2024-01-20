using FlashDotNet.Data;
using FlashDotNet.Enum;
using FlashDotNet.Infrastructure;
using FlashDotNet.Models;
using FlashDotNet.Utils;
using Microsoft.EntityFrameworkCore;

namespace FlashDotNet.Repositories.TestUser;

/// <summary>
/// 和TestUser相关的数据库操作
/// </summary>
public class UserRepository : IUserRepository, IMarkerAddScopedAsImplementedInterfaces
{
    private readonly AppDbContext _context;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="context"></param>
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 判断数据库中用户表是否为空
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsEmptyAsync()
    {
        return !await _context.TestUser.AnyAsync();
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="username"></param>
    /// <param name="argon2Password"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public async Task<UserInfo> CreateUserAsync(string username, string argon2Password, UserRole role = UserRole.User)
    {
        var user = new UserInfo
        {
            Username = username,
            Password = argon2Password,
            Role = role,
        };
        _context.TestUser.Add(user);

        await _context.SaveChangesAsync()
            .TryAsync(new Action<DbUpdateException>(_ => throw new ArgumentException("用户名已存在")));

        return user;
    }

    /// <summary>
    /// 判断密码是否正确
    /// </summary>
    /// <param name="username"></param>
    /// <param name="argon2Password"></param>
    /// <returns></returns>
    public async Task<bool> CheckPasswordAsync(string username, string argon2Password)
    {
        return await _context.TestUser.AsNoTracking().AnyAsync(x =>
            x.Username == username && x.Password == argon2Password);
    }

    /// <summary>
    /// 获取用户角色
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<UserRole> GetUserRoleAsync(string username)
    {
        return await _context.TestUser.AsNoTracking().Where(x => x.Username == username).Select(x => x.Role)
            .FirstAsync();
    }

    /// <summary>
    /// 获取用户角色
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<UserRole> GetUserRoleAsync(int userId)
    {
        return await _context.TestUser.AsNoTracking().Where(x => x.UserId == userId).Select(x => x.Role)
            .FirstAsync();
    }

    /// <summary>
    /// 根据用户名获取用户id
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<int> GetUserIdAsync(string username)
    {
        return await _context.TestUser.AsNoTracking().Where(x => x.Username == username).Select(x => x.UserId)
            .FirstAsync();
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<UserInfo>> GetUserListAsync()
    {
        return await _context.TestUser.AsNoTracking().ToListAsync();
    }
}