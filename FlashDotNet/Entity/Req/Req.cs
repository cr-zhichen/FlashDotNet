using FlashDotNet.Database;

namespace FlashDotNet.Entity.Req;

/// <summary>
/// 请求
/// </summary>
public class Req
{
    /// <summary>
    /// 注册
    /// </summary>
    public class RegisterReq
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = "";

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// 权限
        /// </summary>
        public TestDataBase.UserRole Role { get; set; } = TestDataBase.UserRole.User;
    }
    
    /// <summary>
    /// 登录
    /// </summary>
    public class LoginReq
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = "";

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = "";
    }
}