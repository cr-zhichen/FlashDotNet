using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlashDotNet.Static;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FlashDotNet.Jwt;

// 接口
public interface IJwtService
{
    /// <summary>
    /// 创建令牌
    /// </summary>
    /// <param name="username"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    Task<string> CreateTokenAsync(string username, string role);

    /// <summary>
    /// 验证令牌身份
    /// </summary>
    /// <param name="token"></param>
    /// <param name="requiredRole"></param>
    /// <returns></returns>
    Task<bool> ValidateTokenAsync(string token, string requiredRole = "");

    /// <summary>
    /// 获取令牌中的用户名
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<string> GetUsernameAsync(string token);

    /// <summary>
    /// 注销令牌
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task LogoutAsync(string token);

    /// <summary>
    /// 通过用户ID注销令牌
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task LogoutByUsernameAsync(string username);
}

// 实现
public class JwtService : IJwtService
{
    public Jwt.TokenOptions TokenOptions { get; }

    public JwtService(IOptions<Jwt.TokenOptions> options)
    {
        TokenOptions = options.Value;
    }

    /// <summary>
    /// 创建令牌
    /// </summary>
    /// <param name="username"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public Task<string> CreateTokenAsync(string username, string role)
    {
        // 添加一些需要的键值对
        Claim[] claims = new[]
        {
            new Claim("user", username),
            new Claim("role", role)
        };

        var keyBytes = Encoding.UTF8.GetBytes(TokenOptions.SecretKey);
        var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        var expires = DateTime.Now.AddMinutes(TokenOptions.ExpireMinutes);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: TokenOptions.Issuer, // 签发者
            audience: TokenOptions.Audience, // 接收者
            claims: claims, // payload
            expires: expires, // 过期时间
            signingCredentials: creds); // 令牌

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        TokenList.TokenLists.Add(new TokenList.TokenItem()
        {
            Username = username,
            Token = token,
            ExpireTime = expires
        });
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
        var isValid = TokenList.TokenLists.Any(x => x.Token == token);

        if (!isValid)
        {
            return Task.FromResult(false);
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenOptions.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = TokenOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = TokenOptions.Audience
        };

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            if (requiredRole == "")
            {
                return Task.FromResult(true);
            }

            // Now we get the token and check the role claim
            var jwtToken = (JwtSecurityToken)validatedToken;
            var roleClaim = jwtToken.Claims.First(claim => claim.Type == "role");

            // Check if the role claim is the expected one
            if (roleClaim.Value != requiredRole)
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
    /// 从令牌中获取用户名
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<string> GetUsernameAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenOptions.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = TokenOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = TokenOptions.Audience
        };

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            var userClaim = jwtToken.Claims.First(claim => claim.Type == "user");
            return Task.FromResult(userClaim.Value);
        }
        catch
        {
            return Task.FromResult("");
        }
    }

    /// <summary>
    /// 注销令牌
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task LogoutAsync(string token)
    {
        var tokenToRemove = TokenList.TokenLists.Find(x => x.Token == token);
        if (tokenToRemove != null)
        {
            TokenList.TokenLists.Remove(tokenToRemove);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 根据用户名注销令牌
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public Task LogoutByUsernameAsync(string username)
    {
        var tokenToRemove = TokenList.TokenLists.Find(x => x.Username == username);
        if (tokenToRemove != null)
        {
            TokenList.TokenLists.Remove(tokenToRemove);
        }

        return Task.CompletedTask;
    }
}