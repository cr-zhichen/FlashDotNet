using FlashDotNet.DTOs;
using FlashDotNet.DTOs.HTTP.Requests;
using FlashDotNet.DTOs.HTTP.Responses;

namespace FlashDotNet.Services.UserServer;

/// <summary>
/// 和测试相关的服务接口
/// </summary>
public interface IUserServices
{
    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<IRe<RegisterResponse>> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<IRe<LoginResponse>> LoginAsync(LoginRequest request);

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <returns></returns>
    Task<IRe<List<GetUserListResponse>>> GetUserListAsync();

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IRe<GetUserInfoResponse>> GetUserInfoAsync(string token);
}