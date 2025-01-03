using System.ComponentModel.DataAnnotations;

namespace FlashDotNet.DTOs.HTTP;

/// <summary>
/// 分页请求
/// </summary>
public class PagedRequests
{
    /// <summary>
    /// 页码
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "页码必须大于0")]
    [Required(ErrorMessage = "页码不能为空")]
    public required int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    [Range(1, 100, ErrorMessage = "每页数量必须大于0")]
    [Required(ErrorMessage = "每页数量不能为空")]
    public required int PageSize { get; set; } = 10;
}

/// <summary>
/// 分页响应
/// </summary>
/// <typeparam name="T"></typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// 数据
    /// </summary>
    public required List<T> Data { get; set; }

    /// <summary>
    /// 总数
    /// </summary>
    public required int Total { get; set; }
}
