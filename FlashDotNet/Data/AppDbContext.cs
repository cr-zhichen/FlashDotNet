using FlashDotNet.Enum;
using FlashDotNet.Models;
using FlashDotNet.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FlashDotNet.Data;

/// <summary>
/// 数据库上下文
/// </summary>
public class AppDbContext : DbContext
{
    // 定义全局的 DateTime 转换器
    private readonly static ValueConverter<DateTime, DateTime> DateTimeConverter =
        new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(), // 保存时转换为 UTC
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // 读取时标记为 UTC

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
        base.OnModelCreating(modelBuilder);

        #region UTC时间转换

        // 遍历所有实体类型
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // 遍历所有属性
            foreach (var property in entityType.GetProperties())
            {
                // 如果属性类型是 DateTime 或 DateTime?
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    // 应用全局的 DateTime 转换器
                    property.SetValueConverter(DateTimeConverter);
                }
            }
        }

        #endregion

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
