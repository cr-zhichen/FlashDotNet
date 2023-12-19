using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
    Task<bool> ValidateTokenAsync(string token, string requiredRole = "");

    /// <summary>
    /// 获取令牌中的用户信息
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<UserInfoTokenData> GetUserInfoAsync(string token);

    /// <summary>
    /// 注销令牌
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task LogoutAsync(string token);

    /// <summary>
    /// 根据用户ID注销令牌
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task LogoutByIdAsync(int userId);
}

/// <summary>
/// 表示JWT服务，该服务提供用于创建、验证和管理JWT令牌的功能。
/// </summary>
public class JwtService : IJwtService
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options"></param>
    public JwtService(IOptions<TokenOptions> options)
    {
        TokenOptions = options.Value;
    }

    /// <summary>
    /// 令牌选项
    /// </summary>
    private TokenOptions TokenOptions { get; }

    /// <summary>
    /// 创建令牌
    /// </summary>
    /// <param name="userInfo"></param>
    /// <returns></returns>
    public Task<string> CreateTokenAsync(UserInfoTokenData userInfo)
    {
        // 添加一些需要的键值对
        Claim[] claims =
        {
            new Claim("user_info", JsonConvert.SerializeObject(userInfo)),
        };

        var keyBytes = Encoding.UTF8.GetBytes(TokenOptions.SecretKey);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        // 判断是否设置永不过期
        DateTime? expires = null;
        if (TokenOptions.ExpireMinutes != -1)
        {
            expires = DateTime.UtcNow.AddMinutes(TokenOptions.ExpireMinutes);
        }

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: TokenOptions.Issuer, // 签发者
            audience: TokenOptions.Audience, // 接收者
            claims: claims, // payload
            expires: expires, // 过期时间, 当 ExpireMinutes 为 -1 时，此处为 null，代表无过期时间
            signingCredentials: credentials); // 令牌

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        TokenWhiteList.AddToken(userInfo.UserId, token, expires ?? DateTime.MaxValue);

        return Task.FromResult(token);
    }

    /// <summary>
    /// 验证令牌身份
    /// </summary>
    /// <param name="token"></param>
    /// <param name="requiredRole"></param>
    /// <returns></returns>
    public Task<bool> ValidateTokenAsync(string token, string requiredRole = "")
    {
        //判断令牌是否在缓存中
        var isValid = TokenWhiteList.ContainsToken(token);

        if (!isValid)
        {
            return Task.FromResult(false);
        }

        try
        {
            if (requiredRole == "")
            {
                return Task.FromResult(true);
            }

            UserInfoTokenData userInfo = GetUserInfoAsync(token).Result;

            if (userInfo.Role != requiredRole)
            {
                return Task.FromResult(false);
            }
        }
        catch
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    /// <summary>
    /// 从令牌中获取用户信息
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<UserInfoTokenData> GetUserInfoAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = TokenOptions.ExpireMinutes != -1,
            ValidateIssuerSigningKey = true,
            ValidAudience = TokenOptions.Audience,
            ValidIssuer = TokenOptions.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenOptions.SecretKey))
        };

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            var userInfoClaim = jwtToken.Claims.First(claim => claim.Type == "user_info");

            UserInfoTokenData userInfo = JsonConvert.DeserializeObject<UserInfoTokenData>(userInfoClaim.Value) ??
                                         new UserInfoTokenData();
            return Task.FromResult(userInfo);
        }
        catch
        {
            return Task.FromResult(new UserInfoTokenData());
        }
    }

    /// <summary>
    /// 注销令牌
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task LogoutAsync(string token)
    {
        //从TokenList.TokenLists中移除令牌
        TokenWhiteList.RemoveToken(token);

        return Task.CompletedTask;
    }

    /// <summary>
    /// 根据用户ID注销令牌
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task LogoutByIdAsync(int userId)
    {
        TokenWhiteList.RemoveTokenByUserId(userId);

        return Task.CompletedTask;
    }
}