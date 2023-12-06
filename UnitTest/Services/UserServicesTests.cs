using FlashDotNet.DTOs;
using FlashDotNet.DTOs.HTTP.Requests;
using FlashDotNet.DTOs.HTTP.Responses;
using FlashDotNet.Enum;
using FlashDotNet.Jwt;
using FlashDotNet.Models;
using FlashDotNet.Repositories.TestUser;
using FlashDotNet.Services.TestServices;
using Moq;

namespace UnitTest.Services;

[TestFixture]
public class UserServicesTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IJwtService> _jwtServiceMock = null!;
    private UserServices _userServices = null!;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtServiceMock = new Mock<IJwtService>();
        _userServices = new UserServices(_userRepositoryMock.Object, _jwtServiceMock.Object);
    }

    /// <summary>
    /// 测试UserService类的RegisterAsync方法。
    /// </summary>
    [Test]
    public async Task RegisterAsync_WhenCalled_ReturnsRegisterResponse()
    {
        // Arrange
        var request = new RegisterRequest { Username = "testUser", Password = "password" };
        _userRepositoryMock.Setup(repo => repo.IsEmptyAsync()).ReturnsAsync(false);
        _userRepositoryMock.Setup(repo =>
                repo.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
            .ReturnsAsync(new User { Username = request.Username, Role = UserRole.User });

        _jwtServiceMock.Setup(service => service.CreateTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("token");

        // Act
        var result = await _userServices.RegisterAsync(request);

        // Assert
        Assert.IsInstanceOf<Ok<RegisterResponse>>(result);
        Assert.That(result.Data!.Username, Is.EqualTo("testUser"));
        Assert.That(result.Data.Role, Is.EqualTo("User"));
        Assert.That(result.Data.Token, Is.EqualTo("token"));
    }

    /// <summary>
    /// 测试UserService类的LoginAsync方法。
    /// </summary>
    [Test]
    public async Task LoginAsync_WithCorrectCredentials_ReturnsLoginResponse()
    {
        // Arrange
        var request = new LoginRequest { Username = "testUser", Password = "password" };
        _userRepositoryMock.Setup(repo => repo.CheckPasswordAsync(request.Username, request.Password))
            .ReturnsAsync(true);
        _userRepositoryMock.Setup(repo => repo.GetUserRoleAsync(request.Username)).ReturnsAsync(UserRole.User);
        _jwtServiceMock.Setup(service => service.CreateTokenAsync(request.Username, It.IsAny<string>()))
            .ReturnsAsync("token");

        // Act
        var result = await _userServices.LoginAsync(request);

        // Assert
        Assert.IsInstanceOf<Ok<LoginResponse>>(result);
        Assert.That(result.Data!.Username, Is.EqualTo("testUser"));
        Assert.That(result.Data.Role, Is.EqualTo("User"));
        Assert.That(result.Data.Token, Is.EqualTo("token"));
    }

    /// <summary>
    /// 测试UserService类的LoginAsync方法。
    /// 应该返回错误，因为密码不正确。
    /// </summary>
    [Test]
    public async Task LoginAsync_WithIncorrectCredentials_ReturnsError()
    {
        // Arrange
        var request = new LoginRequest { Username = "testUser", Password = "wrongpassword" };
        _userRepositoryMock.Setup(repo => repo.CheckPasswordAsync(request.Username, request.Password))
            .ReturnsAsync(false);

        // Act
        var result = await _userServices.LoginAsync(request);

        // Assert
        Assert.IsInstanceOf<Error<LoginResponse>>(result);
        Assert.That(result.Message, Is.EqualTo("用户名或密码错误"));
    }

    /// <summary>
    /// 测试UserService类的GetUserListAsync方法。
    /// </summary>
    [Test]
    public async Task GetUserListAsync_WhenCalled_ReturnsUserList()
    {
        // Arrange
        var userList = new List<User>
        {
            new User { UserId = 1, Username = "user1", Role = UserRole.User },
            new User { UserId = 2, Username = "user2", Role = UserRole.Admin }
        };
        _userRepositoryMock.Setup(repo => repo.GetUserListAsync()).ReturnsAsync(userList);

        // Act
        var result = await _userServices.GetUserListAsync();

        // Assert
        Assert.IsInstanceOf<Ok<List<GetUserListResponse>>>(result);
        Assert.That(result.Data!.Count, Is.EqualTo(2));
        Assert.That(result.Data[0].Username, Is.EqualTo("user1"));
        Assert.That(result.Data[0].Role, Is.EqualTo("User"));
        Assert.That(result.Data[1].Username, Is.EqualTo("user2"));
        Assert.That(result.Data[1].Role, Is.EqualTo("Admin"));
    }

    /// <summary>
    /// 测试UserService类的GetUserInfoAsync方法。
    /// </summary>
    [Test]
    public async Task GetUserInfoAsync_WithValidToken_ReturnsUserInfo()
    {
        // Arrange
        string token = "validToken";
        _jwtServiceMock.Setup(service => service.GetUsernameAsync(token)).ReturnsAsync("user1");
        _userRepositoryMock.Setup(repo => repo.GetUserIdAsync("user1")).ReturnsAsync(1);
        _userRepositoryMock.Setup(repo => repo.GetUserRoleAsync(1)).ReturnsAsync(UserRole.User);

        // Act
        var result = await _userServices.GetUserInfoAsync(token);

        // Assert
        Assert.IsInstanceOf<Ok<GetUserInfoResponse>>(result);
        Assert.That(result.Data!.UserId, Is.EqualTo(1));
        Assert.That(result.Data.Username, Is.EqualTo("user1"));
        Assert.That(result.Data.Role, Is.EqualTo("User"));
    }
}