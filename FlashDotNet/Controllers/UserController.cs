using FlashDotNet.Attribute;
using FlashDotNet.DTOs;
using FlashDotNet.DTOs.HTTP.Requests;
using FlashDotNet.DTOs.HTTP.Responses;
using FlashDotNet.Enum;
using FlashDotNet.Jwt;
using FlashDotNet.Services.UserService;
using FlashDotNet.Utils;
using Microsoft.AspNetCore.Mvc;

namespace FlashDotNet.Controllers;

/// <summary>
/// 用户控制器
/// </summary>
[ApiController]
[Route("api/user")]
[Tags("用户")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserController(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
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
    [Auth(UserRole.Admin)]
    [HttpPost("get-user-list")]
    public async Task<IRe<GetUserListResponse>> GetUserListAsync(GetUserListRequest request)
    {
        return await _userService.GetUserListAsync(request);
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns></returns>
    [Auth]
    [HttpPost("get-user-info")]
    public async Task<IRe<GetUserInfoResponse>> GetUserInfoAsync()
    {
        return await _userService.GetUserInfoAsync(await UserHelper.GetCurrentUserIdAsync(_jwtService, Request));
    }

    /// <summary>
    /// 退出登录（吊销令牌）
    /// </summary>
    /// <returns></returns>
    [Auth]
    [HttpPost("get-logout")]
    public async Task<IRe<object>> LogoutAsync()
    {
        return await _userService.LogoutAsync(await UserHelper.GetCurrentUserIdAsync(_jwtService, Request));
    }
}
