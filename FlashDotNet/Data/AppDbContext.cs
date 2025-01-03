using FlashDotNet.Enum;
using FlashDotNet.Models;
using FlashDotNet.Utils;
using Microsoft.EntityFrameworkCore;

namespace FlashDotNet.Data;

/// <summary>
/// 数据库上下文
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options"></param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// 用户表
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// 数据库配置
    /// </summary>
    /// <param name="optionsBuilder"></param>
    override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    /// <summary>
    /// 配置模型
    /// </summary>
    /// <param name="modelBuilder"></param>
    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 配置 UserInfo 表的映射
        modelBuilder
            .Entity<User>(entity =>
            {
                // 配置 Role 属性的转换，使用字符串存储枚举值
                entity
                    .Property(e => e.RoleType)
                    .HasConversion(v => v.GetDisplayName(), v => v.FromDisplayString<UserRole>());
            });
    }
}
