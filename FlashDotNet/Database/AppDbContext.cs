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
    /// 配置数据库
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    /// <summary>
    /// 配置数据库
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}