using FlashDotNet.Models;
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
    /// User 表
    /// </summary>
    public DbSet<User> TestUser { get; set; }

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
        // 配置 User 表的映射
        modelBuilder
            .Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PRIMARY");
                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(e => e.Role).HasConversion<string>();
            });
    }
}