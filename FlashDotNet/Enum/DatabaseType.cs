using System.ComponentModel.DataAnnotations;

namespace FlashDotNet.Enum;

/// <summary>
/// 定义应用程序支持的数据库类型。
/// </summary>
public enum DatabaseType
{
    /// <summary>
    /// MySQL
    /// </summary>
    [Display(Name = "mysql", Description = "MySQL")]
    Mysql,

    /// <summary>
    /// PostgreSQL
    /// </summary>
    [Display(Name = "postgresql", Description = "PostgreSQL")]
    Postgresql,

    /// <summary>
    /// SQLite
    /// </summary>
    [Display(Name = "sqlite", Description = "SQLite")]
    Sqlite,

    /// <summary>
    /// SQL Server
    /// </summary>
    [Display(Name = "sqlserver", Description = "SQL Server")]
    Sqlserver
}