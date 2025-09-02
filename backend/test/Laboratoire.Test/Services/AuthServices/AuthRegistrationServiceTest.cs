using Laboratoire.Application.Services.AuthServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Tests.Services.AuthServices
{
    public class AuthRegistrationServiceTest
    {
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly PasswordHasher _passwordHasher;
        private readonly Mock<ILogger<AuthRegistrationService>> _loggerMock;
        private readonly AuthRegistrationService _service;

        public AuthRegistrationServiceTest()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _loggerMock = new Mock<ILogger<AuthRegistrationService>>();

            var inMemorySettings = new Dictionary<string, string>
            {
                {"AppSettings:PasswordKey", "SuperSecretKey!"}
            };
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            _passwordHasher = new PasswordHasher(config);

            _service = new AuthRegistrationService(
                _authRepositoryMock.Object,
                _passwordHasher,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnConflict_WhenUserAlreadyExists()
        {
            // Arrange
            var user = new UserRegistration { UserId = Guid.NewGuid(), UserPassword = "123" };
            _authRepositoryMock.Setup(a => a.DoesAuthExistsAsync(user.UserId)).ReturnsAsync(true);

            // Act
            var result = await _service.RegisterUserAsync(user);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.NotNull(result.Message);
            Assert.Equal(409, result.StatusCode);
            _authRepositoryMock.Verify(a => a.DoesAuthExistsAsync(user.UserId), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnDbError_WhenAddAuthFails()
        {
            // Arrange
            var user = new UserRegistration { UserId = Guid.NewGuid(), UserPassword = "123" };
            _authRepositoryMock.Setup(a => a.DoesAuthExistsAsync(user.UserId)).ReturnsAsync(false);
            _authRepositoryMock.Setup(a => a.AddAuthAsync(It.IsAny<Auth>())).ReturnsAsync(false);

            // Act
            var result = await _service.RegisterUserAsync(user);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.NotNull(result.Message);
            Assert.Equal(500, result.StatusCode);
            _authRepositoryMock.Verify(a => a.DoesAuthExistsAsync(user.UserId), Times.Once);
            _authRepositoryMock.Verify(a => a.AddAuthAsync(It.IsAny<Auth>()), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnSuccess_WhenRegistrationSucceeds()
        {
            // Arrange
            var user = new UserRegistration { UserId = Guid.NewGuid(), UserPassword = "123" };
            _authRepositoryMock.Setup(a => a.DoesAuthExistsAsync(user.UserId)).ReturnsAsync(false);
            _authRepositoryMock.Setup(a => a.AddAuthAsync(It.IsAny<Auth>())).ReturnsAsync(true);

            // Act
            var result = await _service.RegisterUserAsync(user);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Null(result.Message);
            Assert.Equal(0, result.StatusCode);
            _authRepositoryMock.Verify(a => a.DoesAuthExistsAsync(user.UserId), Times.Once);
            _authRepositoryMock.Verify(a => a.AddAuthAsync(It.IsAny<Auth>()), Times.Once);
        }
    }
}
