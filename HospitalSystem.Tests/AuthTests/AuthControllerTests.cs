using HospitalSystem.Domain.Entities;
using HospitalSystem.Services.AuthService.Interfaces;
using HospitalSystem.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HospitalSystem.Tests.AuthTests
{
    public class AuthControllerTests
    {
        private Mock<IAuthService> CreateMockAuthService(string token)
        {
            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(a => a.AuthenticateUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(token);
            return authServiceMock;
        }

        [Fact]
        public async Task Login_ValidUser_ReturnsToken()
        {
            // Arrange
            var authService = CreateMockAuthService("valid_token");
            var controller = new AuthController(authService.Object);
            var user = new User { Username = "testuser", Password = "testpassword" };

            // Act
            var result = await controller.Login(user);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<string>(okResult.Value?.ToString());
            Assert.Equal("{ token = valid_token }", response);
        }

        [Fact]
        public async Task Login_InvalidUser_ReturnsUnauthorized()
        {
            // Arrange
            var authService = CreateMockAuthService(null);
            var controller = new AuthController(authService.Object);
            var user = new User { Username = "testuser", Password = "invalidpassword" };

            // Act
            var result = await controller.Login(user);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}