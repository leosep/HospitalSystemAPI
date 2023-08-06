using HospitalSystem.DataAccess.Interfaces;
using HospitalSystem.Domain.Entities;
using HospitalSystem.Services.AuthService;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace HospitalSystem.Tests.AuthTests
{
    public class AuthServiceTests
    {
        private IConfiguration GetConfiguration()
        {
            var config = new Mock<IConfiguration>();
            config.Setup(c => c["JwtSettings:SecretKey"]).Returns("JWTAuthenticationHIGHsecuredPassword");
            config.Setup(c => c["JwtSettings:ExpirationInMinutes"]).Returns("120");
            return config.Object;
        }

        private User GetMockUser()
        {
            return new User
            {
                Id = 1, Username = "testuser", Password = "$2a$13$fNz.UjWPOiS9fVI.LQsN4uN6NNvOYIE6lCvyvOpJoyDh3PNyf.iF."
            };
        }

        private Mock<IAuthRepository> CreateMockAuthRepository(User user)
        {
            var mockAuthRepository = new Mock<IAuthRepository>();

            mockAuthRepository.Setup(r => r.GetUserByUsernameAsync("testuser1")).ReturnsAsync(user);

            mockAuthRepository.Setup(r => r.GetUserByUsernameAsync("testuser2")).ReturnsAsync((User)null);

            return mockAuthRepository;
        }

        [Fact]
        public async Task AuthenticateUserAsync_ValidUser_ReturnsToken()
        {
            // Arrange
            var authRepository = CreateMockAuthRepository(GetMockUser());
            var authService = new AuthService(GetConfiguration(), authRepository.Object);

            // Act
            var token = await authService.AuthenticateUserAsync("testuser1", "123456");

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task AuthenticateUserAsync_InvalidUser_ReturnsNull()
        {
            // Arrange
            var authRepository = CreateMockAuthRepository(GetMockUser());
            var authService = new AuthService(GetConfiguration(), authRepository.Object);

            // Act
            var token = await authService.AuthenticateUserAsync("testuser2", "123456");

            // Assert
            Assert.Null(token);
        }

        [Fact]
        public void GenerateJwtToken_ValidUser_ReturnsToken()
        {
            // Arrange
            var authRepository = CreateMockAuthRepository(GetMockUser());
            var authService = new AuthService(GetConfiguration(), authRepository.Object);

            // Act
            var token = authService.GenerateJwtToken("1", "testuser");

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }
    }
}