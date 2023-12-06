using FlashDotNet.Data;
using FlashDotNet.Enum;
using FlashDotNet.Repositories.TestUser;
using FlashDotNet.Utils;
using FlashDotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.Repositories;

[TestFixture]
public class UserRepositoryTests
{
    private AppDbContext _context = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // 使用唯一的数据库名称
            .Options;
        _context = new AppDbContext(options);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted(); // 确保每次测试后删除数据库
        _context.Dispose(); // 释放上下文资源
    }

    // 测试：IsEmptyAsync - 判断用户表是否为空
    [Test]
    public async Task IsEmptyAsync_WithNoUsers_ReturnsTrue()
    {
        // Arrange
        var repository = new UserRepository(_context);

        // Act
        var result = await repository.IsEmptyAsync();

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public async Task IsEmptyAsync_WithUsers_ReturnsFalse()
    {
        await _context.Database.EnsureDeletedAsync();
        // Arrange
        var repository = new UserRepository(_context);
        _context.TestUser.Add(new User { Username = "user1", Password = "password1" });
        await _context.SaveChangesAsync();

        // Act
        var result = await repository.IsEmptyAsync();

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public async Task CreateUserAsync_WhenCalled_CreatesNewUser()
    {
        // Arrange
        var repository = new UserRepository(_context);
        string username = "newuser";
        string password = "newpassword";

        // Act
        var createdUser = await repository.CreateUserAsync(username, password);

        // Assert
        var userInDb = await _context.TestUser.FirstOrDefaultAsync(u => u.Username == username);
        Assert.IsNotNull(userInDb);
        Assert.That(createdUser.Username, Is.EqualTo(username));
        Assert.That(createdUser.Password, Is.EqualTo(password.ToArgon2(username)));
    }

    [Test]
    public async Task CheckPasswordAsync_WithCorrectCredentials_ReturnsTrue()
    {
        // Arrange
        var repository = new UserRepository(_context);
        string username = "user";
        string password = "password";
        _context.TestUser.Add(new User { Username = username, Password = password.ToArgon2(username) });
        await _context.SaveChangesAsync();

        // Act
        var isValid = await repository.CheckPasswordAsync(username, password);

        // Assert
        Assert.IsTrue(isValid);
    }

    [Test]
    public async Task CheckPasswordAsync_WithIncorrectCredentials_ReturnsFalse()
    {
        // Arrange
        var repository = new UserRepository(_context);
        string username = "user";
        string password = "password";
        _context.TestUser.Add(new User { Username = username, Password = password.ToArgon2(username) });
        await _context.SaveChangesAsync();

        // Act
        var isValid = await repository.CheckPasswordAsync(username, "wrongpassword");

        // Assert
        Assert.IsFalse(isValid);
    }

    [Test]
    public async Task GetUserRoleAsync_WithExistingUsername_ReturnsUserRole()
    {
        // Arrange
        var repository = new UserRepository(_context);
        string username = "user";
        UserRole role = UserRole.Admin;
        _context.TestUser.Add(new User { Username = username, Role = role });
        await _context.SaveChangesAsync();

        // Act
        var userRole = await repository.GetUserRoleAsync(username);

        // Assert
        Assert.That(userRole, Is.EqualTo(role));
    }

    [Test]
    public async Task GetUserRoleAsync_WithNonExistingUsername_ReturnsDefaultUserRole()
    {
        // Arrange
        var repository = new UserRepository(_context);
        string username = "nonexistinguser";

        // Act
        var userRole = await repository.GetUserRoleAsync(username);

        // Assert
        Assert.That(userRole, Is.EqualTo(default(UserRole)));
    }

    [Test]
    public async Task GetUserRoleAsync_WithExistingUserId_ReturnsUserRole()
    {
        // Arrange
        var repository = new UserRepository(_context);
        int userId = 1;
        UserRole role = UserRole.Admin;
        _context.TestUser.Add(new User { UserId = userId, Role = role });
        await _context.SaveChangesAsync();

        // Act
        var userRole = await repository.GetUserRoleAsync(userId);

        // Assert
        Assert.That(userRole, Is.EqualTo(role));
    }

    [Test]
    public async Task GetUserRoleAsync_WithNonExistingUserId_ReturnsDefaultUserRole()
    {
        // Arrange
        var repository = new UserRepository(_context);
        int userId = 999;

        // Act
        var userRole = await repository.GetUserRoleAsync(userId);

        // Assert
        Assert.That(userRole, Is.EqualTo(default(UserRole)));
    }

    [Test]
    public async Task GetUserIdAsync_WithExistingUsername_ReturnsUserId()
    {
        // Arrange
        var repository = new UserRepository(_context);
        string username = "user";
        int expectedUserId = 1;
        _context.TestUser.Add(new User { UserId = expectedUserId, Username = username });
        await _context.SaveChangesAsync();

        // Act
        var userId = await repository.GetUserIdAsync(username);

        // Assert
        Assert.That(userId, Is.EqualTo(expectedUserId));
    }

    [Test]
    public async Task GetUserIdAsync_WithNonExistingUsername_ReturnsZero()
    {
        // Arrange
        var repository = new UserRepository(_context);
        string username = "nonexistinguser";

        // Act
        var userId = await repository.GetUserIdAsync(username);

        // Assert
        Assert.That(userId, Is.EqualTo(0));
    }

    [Test]
    public async Task GetUserListAsync_WhenCalled_ReturnsUserList()
    {
        // Arrange
        var repository = new UserRepository(_context);
        _context.TestUser.AddRange(
            new User { Username = "user1" },
            new User { Username = "user2" }
        );
        await _context.SaveChangesAsync();

        // Act
        var users = await repository.GetUserListAsync();

        // Assert
        Assert.That(users.Count, Is.EqualTo(2));
        Assert.IsTrue(users.Any(u => u.Username == "user1"));
        Assert.IsTrue(users.Any(u => u.Username == "user2"));
    }
}