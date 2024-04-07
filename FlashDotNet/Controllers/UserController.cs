using FlashDotNet.Attribute;
using FlashDotNet.DTOs;
using FlashDotNet.DTOs.HTTP.Requests;
using FlashDotNet.DTOs.HTTP.Responses;
using FlashDotNet.Enum;
using FlashDotNet.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace FlashDotNet.Controllers;

/// <summary>
/// 用户控制器
/// </summary>
[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    /// <summary>
    /// 用户服务
    /// </summary>
    private readonly IUserService _userService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userService"></param>
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 注册
    /// </summary>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IRe<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        return await _userService.RegisterAsync(request);
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IRe<LoginResponse>> LoginAsync(LoginRequest request)
    {
        return await _userService.LoginAsync(request);
    }

    /// <summary>
    /// 获取全部用户信息（仅限管理员）
    /// </summary>
    /// <returns></returns>
    [Auth(nameof(UserRole.Admin))]
    [HttpGet("get-user-list")]
    public async Task<IRe<List<GetUserListResponse>>> GetUserListAsync()
    {
        return await _userService.GetUserListAsync();
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns></returns>
    [Auth]
    [HttpGet("get-user-info")]
    public async Task<IRe<GetUserInfoResponse>> GetUserInfoAsync()
    {
        // 获取Token
        var token = Request.Headers["Authorization"].ToString().Split(' ').Last();
        return await _userService.GetUserInfoAsync(token);
    }
}
