using FlashDotNet.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTest.Filter;

[TestFixture]
public class CustomerExceptionFilterTests
{
    private Mock<ILogger<CustomerExceptionFilter>> _loggerMock = null!;
    private CustomerExceptionFilter _filter = null!;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<CustomerExceptionFilter>>();
        _filter = new CustomerExceptionFilter(_loggerMock.Object);
    }

    [Test]
    public async Task OnExceptionAsync_LogsAndHandlesException()
    {
        var exception = new Exception("Test exception");

        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();
        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

        var context = new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = exception
        };

        await _filter.OnExceptionAsync(context);

        _loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    string.Equals("Test exception", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                exception,
                ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
            Times.Once);

        Assert.IsInstanceOf<ContentResult>(context.Result);
        Assert.IsTrue(context.ExceptionHandled);
    }
}