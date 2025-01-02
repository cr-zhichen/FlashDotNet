using FlashDotNet.DTOs;
using FlashDotNet.Enum;
using FlashDotNet.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FlashDotNet.Filter;

/// <summary>
/// 自定义异常过滤器
/// </summary>
public class CustomerExceptionFilter : IAsyncExceptionFilter
{
    private readonly ILogger<CustomerExceptionFilter> _logger;

    /// <summary>
    /// 构造函数注入
    /// </summary>
    /// <param name="logger"></param>
    public CustomerExceptionFilter(ILogger<CustomerExceptionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 重写OnExceptionAsync方法，定义自己的处理逻辑
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task OnExceptionAsync(ExceptionContext context)
    {
        // 如果异常没有被处理则进行处理
        if (context.ExceptionHandled == false)
        {
            _logger.LogError(context.Exception, context.Exception.Message);
            // 定义返回类型
            var result = new Error<object>
            {
                Code = Code.Error,
                Message = context.Exception.Message
            };
            context.Result = new ContentResult
            {
                // 返回状态码设置为200，表示成功
                StatusCode = StatusCodes.Status200OK,
                // 设置返回格式
                ContentType = "application/json;charset=utf-8",
                Content = JsonConvert.SerializeObject(result, JsonConfigurationHelper.GetDefaultSettings())
            };
        }

        // 设置为true，表示异常已经被处理了
        context.ExceptionHandled = true;
        return Task.CompletedTask;
    }
}