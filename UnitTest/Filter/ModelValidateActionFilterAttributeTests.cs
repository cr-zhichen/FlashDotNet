using FlashDotNet.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace UnitTest.Filter;

[TestFixture]
public class ModelValidateActionFilterAttributeTests
{
    private ModelValidateActionFilterAttribute _filter = null!;

    [SetUp]
    public void Setup()
    {
        _filter = new ModelValidateActionFilterAttribute();
    }

    [Test]
    public void OnActionExecuting_InvalidModelState_ReturnsErrorResponse()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Test", "Error Message");

        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor(), modelState);
        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>()!,
            controller: null!);

        _filter.OnActionExecuting(actionExecutingContext);

        Assert.IsInstanceOf<ContentResult>(actionExecutingContext.Result);
        var contentResult = actionExecutingContext.Result as ContentResult;
        Assert.IsNotNull(contentResult);
        Assert.That(contentResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
    }
}