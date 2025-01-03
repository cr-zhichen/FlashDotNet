using FlashDotNet.DTOs;
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
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    /// <inheritdoc />
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
            UserId = user.UserId.ToString(),
            Role = role.GetDisplayName(),
            Version = user.TokenVersion.ToString()
        });

        return new Ok<RegisterResponse>
        {
            Message = "注册成功",
            Data = new RegisterResponse
            {
                Username = user.Username,
                UserId = user.UserId,
                Role = user.RoleType,
                Token = token,
            }
        };
    }

    /// <inheritdoc />
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
        var user = await _userRepository.GetUserAsync(request.Username);

        if (user is null) return new Error<LoginResponse>() { Message = "用户不存在" };

        //生成JWT
        var token = await _jwtService.CreateTokenAsync(new UserInfoTokenData()
        {
            UserId = user.UserId.ToString(),
            Role = user.RoleType.GetDisplayName(),
            Version = user.TokenVersion.ToString()
        });

        return new Ok<LoginResponse>
        {
            Message = "登录成功",
            Data = new LoginResponse
            {
                Username = user.Username,
                UserId = user.UserId,
                Role = user.RoleType,
                Token = token,
            }
        };
    }

    /// <inheritdoc />
    public async Task<IRe<GetUserListResponse>> GetUserListAsync(GetUserListRequest request)
    {
        // 获取用户列表
        var userList = await _userRepository.GetUserListAsync(request.PageIndex, request.PageSize);

        // 转换为DTO
        var response = userList.list.Select(x => new GetUserInfoResponse
        {
            UserId = x.UserId,
            Username = x.Username,
            Role = x.RoleType,
        }).ToList();

        return new Ok<GetUserListResponse>
        {
            Message = "获取成功",
            Data = new GetUserListResponse()
            {
                Total = userList.total,
                Data = response
            }
        };
    }

    /// <inheritdoc />
    public async Task<IRe<GetUserInfoResponse>> GetUserInfoAsync(Guid userId)
    {
        var user = await _userRepository.GetUserAsync(userId);

        if (user is null) return new Error<GetUserInfoResponse>() { Message = "用户不存在" };

        return new Ok<GetUserInfoResponse>
        {
            Message = "获取成功",
            Data = new GetUserInfoResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.RoleType
            }
        };
    }
}
