using FlashDotNet.DTOs;
using FlashDotNet.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FlashDotNet.Filter;

/// <summary>
/// 模型验证过滤器
/// </summary>
public class ModelValidateActionFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            //获取验证失败的模型字段
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .Select(e => e.Value.Errors.First().ErrorMessage)
                .ToList();

            var str = string.Join("|", errors);

            var result = new Ok<object>
            {
                Code = Code.Error,
                Message = "未通过数据验证，请检查传入参数",
            };

            context.Result = new ContentResult
            {
                // 返回状态码设置为200，表示成功
                StatusCode = StatusCodes.Status200OK,
                // 设置返回格式
                ContentType = "application/json;charset=utf-8",
                Content = JsonConvert.SerializeObject(result, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                })
            };
        }
    }
}
