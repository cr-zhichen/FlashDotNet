using FlashDotNet.Controllers;
using FlashDotNet.DTOs;
using FlashDotNet.DTOs.HTTP.Requests;
using FlashDotNet.DTOs.HTTP.Responses;
using FlashDotNet.Services.TestServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private UserController _controller = null!;
        private Mock<IUserServices> _userServicesMock = null!;

        [SetUp]
        public void Setup()
        {
            _userServicesMock = new Mock<IUserServices>();
            _controller = new UserController(_userServicesMock.Object);
        }

        [Test]
        public async Task RegisterAsync_ValidRequest_ReturnsResponse()
        {
            // Arrange
            var request = new RegisterRequest();
            var expectedResult = new Ok<RegisterResponse> { Data = new RegisterResponse() };

            _userServicesMock.Setup(x => x.RegisterAsync(request))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.RegisterAsync(request);

            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedResult.Data));
            Assert.That(result.Code, Is.EqualTo(expectedResult.Code));
            Assert.That(result.Message, Is.EqualTo(expectedResult.Message));
        }

        [Test]
        public async Task LoginAsync_ValidRequest_ReturnsResponse()
        {
            // Arrange
            var request = new LoginRequest();
            var expectedResult = new Ok<LoginResponse> { Data = new LoginResponse() };

            _userServicesMock.Setup(x => x.LoginAsync(request))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.LoginAsync(request);

            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedResult.Data));
            Assert.That(result.Code, Is.EqualTo(expectedResult.Code));
            Assert.That(result.Message, Is.EqualTo(expectedResult.Message));
        }

        [Test]
        public async Task GetUserListAsync_AuthorizedUser_ReturnsResponse()
        {
            // Arrange
            var expectedResult = new Ok<List<GetUserListResponse>> { Data = new List<GetUserListResponse>() };

            _userServicesMock.Setup(x => x.GetUserListAsync())
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetUserListAsync();

            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedResult.Data));
            Assert.That(result.Code, Is.EqualTo(expectedResult.Code));
            Assert.That(result.Message, Is.EqualTo(expectedResult.Message));
        }

        [Test]
        public async Task GetUserInfoAsync_ValidToken_ReturnsResponse()
        {
            // Arrange
            var token = "valid_token";
            var expectedResult = new Ok<GetUserInfoResponse> { Data = new GetUserInfoResponse() };

            // Mock the request headers using ControllerContext
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
                {
                    Request = { Headers = { { "Authorization", $"Bearer {token}" } } }
                }
            };

            _userServicesMock.Setup(x => x.GetUserInfoAsync(token))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetUserInfoAsync();

            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedResult.Data));
            Assert.That(result.Code, Is.EqualTo(expectedResult.Code));
            Assert.That(result.Message, Is.EqualTo(expectedResult.Message));
        }
    }
}