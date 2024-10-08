﻿using FlashDotNet.DTOs;
using FlashDotNet.DTOs.HTTP.Requests;
using FlashDotNet.DTOs.HTTP.Responses;
using FlashDotNet.Enum;
using FlashDotNet.Infrastructure;
using FlashDotNet.Jwt;
using FlashDotNet.Repositories.UserRepository;
using FlashDotNet.Utils;

namespace FlashDotNet.Services.UserService;

/// <summary>
/// 和用户相关的服务
/// </summary>
[AddScopedAsImplementedInterfaces]
public class UserService : IUserService
{
    /// <summary>
    /// JWT相关的服务
    /// </summary>
    private readonly IJwtService _jwtService;

    /// <summary>
    /// 用户相关的数据库操作
    /// </summary>
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userRepository"></param>
    /// <param name="jwtService"></param>
    public UserService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IRe<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        UserRole role = UserRole.User;

        //如果数据库中没有用户，那么注册的第一个用户就是管理员
        if (await _userRepository.IsEmptyAsync())
        {
            role = UserRole.Admin;
        }

        //创建用户
        var user = await _userRepository
            .CreateUserAsync(request.Username, request.Password.ToArgon2(request.Username), role);

        //生成JWT
        var token = await _jwtService.CreateTokenAsync(new UserInfoTokenData()
        {
            UserId = user.UserId,
            Username = user.Username,
            Role = role.GetDisplayName()
        });

        return new Ok<RegisterResponse>
        {
            Message = "注册成功",
            Data = new RegisterResponse
            {
                Username = user.Username,
                Role = user.Role.GetDisplayName(),
                Token = token
            }
        };
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IRe<LoginResponse>> LoginAsync(LoginRequest request)
    {
        // 判断密码是否正确
        if (!await _userRepository.CheckPasswordAsync(request.Username, request.Password.ToArgon2(request.Username)))
        {
            return new Error<LoginResponse>
            {
                Message = "用户名或密码错误"
            };
        }

        //获取用户id
        var userId = await _userRepository.GetUserIdAsync(request.Username);

        //获取用户角色
        var role = await _userRepository.GetUserRoleAsync(userId);

        //生成JWT
        var token = await _jwtService.CreateTokenAsync(new UserInfoTokenData()
        {
            UserId = userId,
            Username = request.Username,
            Role = role.GetDisplayName()
        });

        return new Ok<LoginResponse>
        {
            Message = "登录成功",
            Data = new LoginResponse
            {
                Username = request.Username,
                Role = role.GetDisplayName(),
                Token = token
            }
        };
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <returns></returns>
    public async Task<IRe<List<GetUserListResponse>>> GetUserListAsync()
    {
        // 获取用户列表
        var userList = await _userRepository.GetUserListAsync();

        // 转换为DTO
        var response = userList.Select(x => new GetUserListResponse
        {
            UserId = x.UserId,
            Username = x.Username,
            Role = x.Role.GetDisplayName(),
        }).ToList();

        return new Ok<List<GetUserListResponse>>
        {
            Message = "获取成功",
            Data = response
        };
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<IRe<GetUserInfoResponse>> GetUserInfoAsync(string token)
    {
        // 从令牌中获取用户名
        var userInfo = await _jwtService.GetUserInfoAsync(token);

        // 获取用户角色
        var role = await _userRepository.GetUserRoleAsync(userInfo!.UserId);

        return new Ok<GetUserInfoResponse>
        {
            Message = "获取成功",
            Data = new GetUserInfoResponse
            {
                UserId = userInfo.UserId,
                Username = userInfo.Username,
                Role = role.GetDisplayName(),
            }
        };
    }
}
