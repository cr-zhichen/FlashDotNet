using Microsoft.EntityFrameworkCore;

namespace FlashDotNet.Database;

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
    /// TestDataBase 表
    /// </summary>
    public DbSet<TestDataBase> TestDataBases { get; set; }

    /// <summary>
    /// 数据库配置
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    /// <summary>
    /// 配置模型
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 配置 TestDataBase 表中的Role字段 枚举类型与字符串类型的转换
        modelBuilder
            .Entity<TestDataBase>()
            .Property(e => e.Role)
            .HasConversion<string>();
    }
}