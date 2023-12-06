using FlashDotNet.Jwt;
using FlashDotNet.Static;
using Microsoft.Extensions.Options;
using Moq;

namespace UnitTest.Jwt;

[TestFixture]
public class JwtServiceTests
{
    private JwtService _jwtService = null!;
    private Mock<IOptions<TokenOptions>> _optionsMock = null!;

    [SetUp]
    public void Setup()
    {
        // 创建模拟的Jwt.TokenOptions
        var tokenOptions = new TokenOptions
        {
            SecretKey = "C1slpZJycNlKjE7Y5jAFM5IqVnojZwJyvMNQ2LzsVqdR8smwTP",
            Issuer = "zFAgkPdNz51yb4VtAIEk65AeKKZuaK2syQXvdrzVUqip6o36WH",
            Audience = "SQkfKpOLVrBWglgecGjhg1Xx5YiY1rs1Onr2ZbYmq8RzXfDKm4",
            ExpireMinutes = 60 // 设置适当的过期时间
        };

        // 使用Moq创建IOptions<Jwt.TokenOptions>的模拟
        _optionsMock = new Mock<IOptions<TokenOptions>>();
        _optionsMock.Setup(x => x.Value).Returns(tokenOptions);

        // 初始化JwtService实例
        _jwtService = new JwtService(_optionsMock.Object);
    }


    [Test]
    public async Task CreateTokenAsync_ValidInput_ReturnsValidToken()
    {
        // 准备测试数据
        string username = "testuser";
        string role = "admin";

        // 调用方法
        string token = await _jwtService.CreateTokenAsync(username, role);

        // 断言
        Assert.IsNotNull(token);
        Assert.IsNotEmpty(token);
    }

    [Test]
    public async Task ValidateTokenAsync_ValidToken_ReturnsTrue()
    {
        // 准备测试数据
        string username = "testuser";
        string role = "admin";
        string token = await _jwtService.CreateTokenAsync(username, role);

        // 调用方法
        bool isValid = await _jwtService.ValidateTokenAsync(token, role);

        // 断言
        Assert.IsTrue(isValid);
    }

    [Test]
    public async Task GetUsernameAsync_ValidToken_ReturnsUsername()
    {
        // 准备测试数据
        string username = "testuser";
        string role = "admin";
        string token = await _jwtService.CreateTokenAsync(username, role);

        // 调用方法
        string resultUsername = await _jwtService.GetUsernameAsync(token);

        // 断言
        Assert.That(resultUsername, Is.EqualTo(username));
    }

    [Test]
    public async Task LogoutAsync_ValidToken_RemovesToken()
    {
        // 准备测试数据
        string username = "testuser";
        string role = "admin";
        string token = await _jwtService.CreateTokenAsync(username, role);

        // 调用方法前先确保token已经被添加到缓存中
        // 示例：TokenList.TokenLists.Add(new TokenList.TokenItem() { Username = username, Token = token });

        // 调用方法
        await _jwtService.LogoutAsync(token);
        bool isValid = await _jwtService.ValidateTokenAsync(token, role);

        // 断言
        Assert.IsFalse(isValid);
    }

    [Test]
    public async Task LogoutByUsernameAsync_ValidUsername_RemovesToken()
    {
        // 准备测试数据
        string username = "testuser";
        string role = "admin";
        await _jwtService.CreateTokenAsync(username, role);

        // 调用方法前先确保token已经被添加到缓存中
        // 示例：TokenList.TokenLists.Add(new TokenList.TokenItem() { Username = username, Token = token });

        // 调用方法
        await _jwtService.LogoutByUsernameAsync(username);
        bool isValid = await _jwtService.ValidateTokenAsync(username, role);

        // 断言
        Assert.IsFalse(isValid);
    }

    [Test]
    public async Task ValidateTokenAsync_NoRequiredRole_ReturnsTrue()
    {
        // 准备测试数据
        string username = "testuser";
        string role = "admin";
        string token = await _jwtService.CreateTokenAsync(username, role);

        // 调用方法
        bool isValid = await _jwtService.ValidateTokenAsync(token);

        // 断言
        Assert.IsTrue(isValid);
    }

    [Test]
    public async Task ValidateTokenAsync_InvalidRole_ReturnsFalse()
    {
        // 准备测试数据
        string username = "testuser";
        string role = "user"; // 这里使用一个不匹配的角色
        string token = await _jwtService.CreateTokenAsync(username, "admin"); // 创建一个带有"admin"角色的令牌

        // 调用方法
        bool isValid = await _jwtService.ValidateTokenAsync(token, role);

        // 断言
        Assert.IsFalse(isValid);
    }

    [Test]
    public async Task ValidateTokenAsync_InvalidToken_ReturnsFalse()
    {
        // 准备测试数据
        string invalidToken = "invalidtoken"; // 一个无效的令牌

        // 调用方法
        bool isValid = await _jwtService.ValidateTokenAsync(invalidToken);

        // 断言
        Assert.IsFalse(isValid);
    }
}