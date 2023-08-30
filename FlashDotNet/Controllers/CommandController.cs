using FlashDotNet.Database;
using FlashDotNet.Entity.Re;
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
    /// 构造函数
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    public CommandController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// 测试
    /// </summary>
    /// <returns></returns>
    [HttpPost("test")]
    public async Task<IRe<object>> Test()
    {
        return new Ok<object>()
        {
            Message = "test",
            Code = 0,
            Data = new
            {
                Name = "test"
            }
        };
    }
}