using FlashDotNet.Data;
using FlashDotNet.Enum;
using FlashDotNet.Utils;
using Microsoft.EntityFrameworkCore;

namespace FlashDotNet.Repositories.TestUser;

/// <summary>
/// 和TestUser相关的数据库操作
/// </summary>
public class UserRepository : IUserRepository
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
        return await _context.TestUser.AnyAsync();
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public async Task<Models.User> CreateUserAsync(string username, string password, UserRole role = UserRole.User)
    {
        var user = new Models.User
        {
            Username = username,
            Password = password.ToArgon2(username),
            Role = role,
        };
        _context.TestUser.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// 判断密码是否正确
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<bool> CheckPasswordAsync(string username, string password)
    {
        return await _context.TestUser.AnyAsync(x =>
            x.Username == username && x.Password == password.ToArgon2(username));
    }

    /// <summary>
    /// 获取用户角色
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<UserRole> GetUserRoleAsync(string username)
    {
        return await _context.TestUser.Where(x => x.Username == username).Select(x => x.Role)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// 获取用户角色
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<UserRole> GetUserRoleAsync(int userId)
    {
        return await _context.TestUser.Where(x => x.UserId == userId).Select(x => x.Role)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// 根据用户名获取用户id
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<int> GetUserIdAsync(string username)
    {
        return await _context.TestUser.Where(x => x.Username == username).Select(x => x.UserId)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<Models.User>> GetUserListAsync()
    {
        return await _context.TestUser.ToListAsync();
    }
}