using FlashDotNet.Attribute;
using FlashDotNet.Database;
using FlashDotNet.Entity.Re;
using FlashDotNet.Entity.Req;
using FlashDotNet.Jwt;
using FlashDotNet.Utils;
using Microsoft.AspNetCore.Mvc;

namespace FlashDotNet.Controllers;

/// <summary>
/// 命令控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class CommandController : ControllerBase
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// 配置文件
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// JWT服务
    /// </summary>
    private readonly IJwtService _jwtService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="jwtService"></param>
    public CommandController(AppDbContext context, IConfiguration configuration, IJwtService jwtService)
    {
        _context = context;
        _configuration = configuration;
        _jwtService = jwtService;
    }

    /// <summary>
    /// 注册
    /// </summary>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IRe<string>> Register(Req.RegisterReq data)
    {
        if (_context.TestDataBases.Any(x => x.Username == data.Username))
        {
            return new Error<string>()
            {
                Message = "用户名已存在"
            };
        }

        var token = _jwtService.CreateTokenAsync(data.Username, data.Role.ToString());
        _context.TestDataBases.Add(new TestDataBase()
        {
            Username = data.Username,
            Password = data.Password.ToArgon2(),
            Role = data.Role
        });
        await _context.SaveChangesAsync();
        return new Ok<string>()
        {
            Message = "注册成功",
            Data = token.Result
        };
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IRe<object>> Login(Req.LoginReq data)
    {
        var user = _context.TestDataBases.FirstOrDefault(x => x.Username == data.Username);
        if (user == null)
        {
            return new Error<object>()
            {
                Message = "用户不存在"
            };
        }

        if (user.Password == data.Password.ToArgon2())
        {
            var token = _jwtService.CreateTokenAsync(data.Username, user.Role.ToString());
            return new Ok<object>()
            {
                Message = "登录成功",
                Data = new
                {
                    Token = token.Result,
                    Role = user.Role
                }
            };
        }

        return new Error<object>()
        {
            Message = "密码错误"
        };
    }

    /// <summary>
    /// 获取全部用户信息（仅限管理员）
    /// </summary>
    /// <returns></returns>
    [Auth(nameof(TestDataBase.UserRole.Admin))]
    [HttpGet("getAll")]
    public async Task<IRe<List<TestDataBase>>> GetAll()
    {
        return new Ok<List<TestDataBase>>()
        {
            Message = "获取全部用户信息成功",
            Data = _context.TestDataBases.ToList()
        };
    }
}