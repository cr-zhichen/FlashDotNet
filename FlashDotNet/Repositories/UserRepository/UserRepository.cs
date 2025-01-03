using FlashDotNet.Data;
using FlashDotNet.Enum;
using FlashDotNet.Infrastructure;
using FlashDotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashDotNet.Repositories.UserRepository;

/// <inheritdoc />
[AddScopedAsImplementedInterfaces]
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<bool> IsEmptyAsync()
    {
        return !await _context.Users.AnyAsync();
    }

    /// <inheritdoc />
    public async Task<User> CreateUserAsync(string username, string argon2Password, UserRole role = UserRole.User)
    {
        // 判断用户是否存在
        if (await _context.Users.AnyAsync(x => x.Username == username))
        {
            throw new UserAlreadyExistsException(username);
        }

        var user = new User
        {
            Username = username,
            PasswordHash = argon2Password,
            RoleType = role,
            TokenVersion = Guid.NewGuid()
        };
        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }

    /// <inheritdoc />
    public async Task<bool> CheckPasswordAsync(string username, string argon2Password)
    {
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(x => x.Username == username && x.PasswordHash == argon2Password);
    }

    /// <inheritdoc />
    public async Task<User?> GetUserAsync(string username)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username);
    }

    /// <inheritdoc />
    public async Task<User?> GetUserAsync(Guid userId)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    /// <inheritdoc />
    public async Task<List<User>> GetUserListAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .ToListAsync();
    }

    #region 错误处理

    /// <summary>
    /// 用户已存在
    /// </summary>
    private class UserAlreadyExistsException : UserRepositoryException
    {
        /// <inheritdoc />
        public UserAlreadyExistsException(string username) : base($"用户名 '{username}' 已存在") { }
    }

    /// <inheritdoc />
    private class UserRepositoryException : RepositoryException
    {
        /// <inheritdoc />
        protected UserRepositoryException(string message) : base(message) { }
    }

    #endregion
}
