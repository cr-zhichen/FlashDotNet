using FlashDotNet.Attribute;
using FlashDotNet.DTOs;
using FlashDotNet.DTOs.HTTP.Requests;
using FlashDotNet.DTOs.HTTP.Responses;
using FlashDotNet.Enum;
using FlashDotNet.Services.TestServices;
using Microsoft.AspNetCore.Mvc;

namespace FlashDotNet.Controllers;

/// <summary>
/// 命令控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    /// <summary>
    /// 用户服务
    /// </summary>
    private readonly ITestServices _testServices;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="testServices"></param>
    public TestController(ITestServices testServices)
    {
        _testServices = testServices;
    }

    /// <summary>
    /// 注册
    /// </summary>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IRe<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        return await _testServices.RegisterAsync(request);
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IRe<LoginResponse>> LoginAsync(LoginRequest request)
    {
        return await _testServices.LoginAsync(request);
    }

    /// <summary>
    /// 获取全部用户信息（仅限管理员）
    /// </summary>
    /// <returns></returns>
    [Auth(nameof(UserRole.Admin))]
    [HttpGet("getAll")]
    public async Task<IRe<List<GetUserListResponse>>> GetUserListAsync()
    {
        return await _testServices.GetUserListAsync();
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns></returns>
    [Auth]
    [HttpGet("getInfo")]
    public async Task<IRe<GetUserInfoResponse>> GetUserInfoAsync()
    {
        // 获取Token
        var token = Request.Headers["Authorization"].ToString().Split(' ').Last();
        return await _testServices.GetUserInfoAsync(token);
    }
}