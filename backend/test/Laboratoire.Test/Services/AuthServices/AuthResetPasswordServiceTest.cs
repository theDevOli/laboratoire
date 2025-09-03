using System.Security.Cryptography;
using Laboratoire.Application.Services.AuthServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.AuthServices
{
    public class AuthResetPasswordServiceTest
    {
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly PasswordHasher _passwordHasher;
        private readonly Mock<ILogger<AuthResetPasswordService>> _loggerMock;
        private readonly AuthResetPasswordService _service;

        public AuthResetPasswordServiceTest()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _loggerMock = new Mock<ILogger<AuthResetPasswordService>>();

            var inMemorySettings = new Dictionary<string, string>
            {
                {"AppSettings:PasswordKey", "SuperSecretKey!"}
            };
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            _passwordHasher = new PasswordHasher(config);

            _service = new AuthResetPasswordService(
                _authRepositoryMock.Object,
                _passwordHasher,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _authRepositoryMock.Setup(a => a.GetAuthByUserIdAsync(userId)).ReturnsAsync((Auth?)null);

            // Act
            var result = await _service.ResetPasswordAsync(userId);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.NotNull(result.Message);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldReturnDbError_WhenUpdateFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var salt = new byte[16];
            RandomNumberGenerator.Fill(salt);
            var auth = new Auth { UserId = userId, PasswordSalt = salt, PasswordHash = salt };
            
            _authRepositoryMock.Setup(a => a.GetAuthByUserIdAsync(userId)).ReturnsAsync(auth);
            _authRepositoryMock.Setup(a => a.UpdateAuthAsync(It.IsAny<Auth>())).ReturnsAsync(false);

            // Act
            var result = await _service.ResetPasswordAsync(userId);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.NotNull(result.Message);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldReturnSuccess_WhenPasswordResetSucceeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var salt = new byte[16];
            RandomNumberGenerator.Fill(salt);
            var auth = new Auth { UserId = userId, PasswordSalt = salt, PasswordHash = salt };
            
            _authRepositoryMock.Setup(a => a.GetAuthByUserIdAsync(userId)).ReturnsAsync(auth);
            _authRepositoryMock.Setup(a => a.UpdateAuthAsync(It.IsAny<Auth>())).ReturnsAsync(true);

            // Act
            var result = await _service.ResetPasswordAsync(userId);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Null(result.Message);
            Assert.Equal(0, result.StatusCode); 
            _authRepositoryMock.Verify(a => a.UpdateAuthAsync(It.IsAny<Auth>()), Times.Once);
        }
    }
}
