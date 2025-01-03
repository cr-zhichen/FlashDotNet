using FlashDotNet.DTOs;
using FlashDotNet.DTOs.HTTP.Requests;
using FlashDotNet.DTOs.HTTP.Responses;

namespace FlashDotNet.Services.UserService;

/// <summary>
/// 和测试相关的服务接口
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="request">注册请求</param>
    /// <returns>注册响应</returns>
    Task<IRe<RegisterResponse>> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>登录响应</returns>
    Task<IRe<LoginResponse>> LoginAsync(LoginRequest request);

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="request">获取用户请求</param>
    /// <returns>用户列表响应</returns>
    Task<IRe<GetUserListResponse>> GetUserListAsync(GetUserListRequest request);

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户信息响应</returns>
    Task<IRe<GetUserInfoResponse>> GetUserInfoAsync(Guid userId);
}
