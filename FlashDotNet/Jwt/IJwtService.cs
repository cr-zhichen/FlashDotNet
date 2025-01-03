using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlashDotNet.Data;
using FlashDotNet.Infrastructure;
using FlashDotNet.Services.CacheService;
using FlashDotNet.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FlashDotNet.Jwt;

/// <summary>
/// 表示JWT服务，该服务提供用于创建、验证和管理JWT令牌的功能。
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// 创建令牌
    /// </summary>
    /// <param name="userInfo"></param>
    /// <returns></returns>
    Task<string> CreateTokenAsync(UserInfoTokenData userInfo);

    /// <summary>
    /// 验证令牌身份
    /// </summary>
    /// <param name="token"></param>
    /// <param name="requiredRole"></param>
    /// <returns></returns>
    Task<(bool IsValid, string? ErrorMessage)> ValidateTokenAsync(string token, string requiredRole = "");

    /// <summary>
    /// 获取令牌中的用户信息
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<UserInfoTokenData?> GetUserInfoAsync(string token);

    /// <summary>
    /// 根据用户ID注销令牌
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task LogoutByIdAsync(string userId);
}

/// <summary>
/// 表示JWT服务，该服务提供用于创建、验证和管理JWT令牌的功能。
/// </summary>
[AddTransientAsImplementedInterfaces]
public class JwtService : IJwtService
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly AppDbContext _appDbContext;

    /// <summary>
    /// 令牌选项
    /// </summary>
    private readonly TokenOptions _tokenOptions;

    /// <summary>
    /// 缓存服务
    /// </summary>
    private readonly ICacheService _cacheService;

    /// <summary>
    /// 构造函数
    /// </summary>
    public JwtService(IOptions<TokenOptions> options, AppDbContext appDbContext, ICacheService cacheService)
    {
        _appDbContext = appDbContext;
        _cacheService = cacheService;
        _tokenOptions = options.Value;
    }

    /// <summary>
    /// Token版本号键
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private string TokenVersionKey(string userId) => $"TokenVersion_{userId}";

    /// <summary>
    /// 创建令牌
    /// </summary>
    /// <param name="userInfo"></param>
    /// <returns></returns>
    public Task<string> CreateTokenAsync(UserInfoTokenData userInfo)
    {
        // 添加一些需要的键值对
        Claim[] claims =
        [
            new Claim("user_info", JsonConvert.SerializeObject(userInfo))
        ];

        var keyBytes = Encoding.UTF8.GetBytes(_tokenOptions.SecretKey);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        // 判断是否设置永不过期
        DateTime? expires = null;
        if (_tokenOptions.ExpireMinutes != -1)
        {
            expires = DateTime.UtcNow.AddMinutes(_tokenOptions.ExpireMinutes);
        }

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer, // 签发者
            audience: _tokenOptions.Audience, // 接收者
            claims: claims, // payload
            expires: expires, // 过期时间, 当 ExpireMinutes 为 -1 时，此处为 null，代表无过期时间
            signingCredentials: credentials); // 令牌

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return Task.FromResult(token);
    }

    /// <summary>
    /// 验证令牌身份
    /// </summary>
    /// <param name="token"></param>
    /// <param name="requiredRole"></param>
    /// <returns></returns>
    public async Task<(bool IsValid, string? ErrorMessage)> ValidateTokenAsync(string token, string requiredRole = "")
    {
        try
        {
            UserInfoTokenData? userInfo = await GetUserInfoAsync(token);
            if (userInfo == null)
            {
                return (false, "无效的令牌：未找到用户信息。");
            }

            if (!string.IsNullOrEmpty(requiredRole) && userInfo.Role != requiredRole)
            {
                return (false, $"无效的令牌：需要 {requiredRole} 角色。");
            }

            Guid? tokenVersion = _cacheService.Get<Guid>(TokenVersionKey(userInfo.UserId));

            if (tokenVersion == null || tokenVersion == Guid.Empty)
            {
                var user = await _appDbContext.Users.FindAsync(userInfo.UserId.ToGuid());
                if (user == null)
                {
                    return (false, "无效的令牌：未找到用户。");
                }

                tokenVersion = user.TokenVersion;
                _cacheService.Set(TokenVersionKey(userInfo.UserId), tokenVersion);
            }

            if (userInfo.Version.ToGuid() != tokenVersion)
            {
                return (false, "无效的令牌：令牌版本不匹配。");
            }
        }
        catch (Exception ex)
        {
            return (false, $"无效的令牌：{ex.Message}");
        }

        return (true, null);
    }

    /// <summary>
    /// 从令牌中获取用户信息
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<UserInfoTokenData?> GetUserInfoAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = _tokenOptions.ExpireMinutes != -1,
            ValidateIssuerSigningKey = true,
            ValidAudience = _tokenOptions.Audience,
            ValidIssuer = _tokenOptions.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecretKey))
        };

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            var userInfoClaim = jwtToken.Claims.First(claim => claim.Type == "user_info");

            UserInfoTokenData userInfo = JsonConvert.DeserializeObject<UserInfoTokenData>(userInfoClaim.Value) ?? throw new Exception("无法解析用户信息");
            return Task.FromResult<UserInfoTokenData?>(userInfo);
        }
        catch
        {
            return Task.FromResult<UserInfoTokenData?>(null);
        }
    }

    /// <summary>
    /// 根据用户ID注销令牌
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task LogoutByIdAsync(string userId)
    {
        // 通过主键获取用户
        var user = _appDbContext.Users.Find(userId.ToGuid());

        // 清除缓存
        _cacheService.Remove(TokenVersionKey(userId));

        if (user != null)
        {
            user.TokenVersion = Guid.NewGuid();
            _appDbContext.SaveChanges();

            // 更新缓存中的TokenVersion
            _cacheService.Set(TokenVersionKey(userId), user.TokenVersion);
        }

        return Task.CompletedTask;
    }
}

