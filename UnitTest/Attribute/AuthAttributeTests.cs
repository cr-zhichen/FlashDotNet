using FlashDotNet.Attribute;
using FlashDotNet.DTOs;
using FlashDotNet.Enum;
using FlashDotNet.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace UnitTest.Attribute;

[TestFixture]
public class AuthAttributeTests
{
    private Mock<IJwtService> _jwtServiceMock = null!;
    private AuthAttribute _authAttribute = null!;

    [SetUp]
    public void Setup()
    {
        _jwtServiceMock = new Mock<IJwtService>();
        _authAttribute = new AuthAttribute("Role");
    }

    [Test]
    public async Task OnActionExecutionAsync_ValidToken_ContinuesExecution()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "Bearer validToken";

        _jwtServiceMock.Setup(service => service.ValidateTokenAsync("validToken", "Role"))
            .ReturnsAsync(true);

        var context = new ActionExecutingContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object>()!,
            controller: null!);

        var next = new Mock<ActionExecutionDelegate>();

        httpContext.RequestServices = new ServiceCollection()
            .AddSingleton(_jwtServiceMock.Object)
            .BuildServiceProvider();

        await _authAttribute.OnActionExecutionAsync(context, next.Object);

        next.Verify(execution => execution(), Times.Once());
    }

    [Test]
    public async Task OnActionExecutionAsync_InvalidToken_ReturnsError()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "Bearer invalidToken";

        _jwtServiceMock.Setup(service => service.ValidateTokenAsync("invalidToken", "Role"))
            .ReturnsAsync(false);

        var context = new ActionExecutingContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object>()!,
            controller: null!);

        httpContext.RequestServices = new ServiceCollection()
            .AddSingleton(_jwtServiceMock.Object)
            .BuildServiceProvider();

        await _authAttribute.OnActionExecutionAsync(context, null);

        var result = context.Result as JsonResult;
        Assert.IsNotNull(result);
        Assert.That(result!.StatusCode, Is.EqualTo(200));

        var errorObject = result.Value as Error<object>;
        Assert.IsNotNull(errorObject);
        Assert.That(errorObject!.Code, Is.EqualTo(Code.TokenError));
        Assert.That(errorObject.Message, Is.EqualTo("Token验证失败"));
    }
}