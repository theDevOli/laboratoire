using System.Security.Cryptography;
using Laboratoire.Application.Services.AuthServices;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.AuthServices
{
    public class AuthLoginServiceTest
    {
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<IUserGetterByUsernameService> _userGetterMock;
        private readonly PasswordHasher _passwordHasher;
        private readonly Mock<ILogger<AuthLoginService>> _loggerMock;
        private readonly AuthLoginService _service;

        public AuthLoginServiceTest()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _userGetterMock = new Mock<IUserGetterByUsernameService>();
            _loggerMock = new Mock<ILogger<AuthLoginService>>();

            var inMemorySettings = new Dictionary<string, string>
            {
                {"AppSettings:PasswordKey", "SuperSecretKey!"}
            };
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            _passwordHasher = new PasswordHasher(config);

            _service = new AuthLoginService(
                _authRepositoryMock.Object,
                _passwordHasher,
                _userGetterMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var login = new UserLogin { Username = "unknown", UserPassword = "123" };
            _userGetterMock.Setup(u => u.GetUserByUsernameAsync(login.Username)).ReturnsAsync((User?)null);

            // Act
            var result = await _service.LoginUserAsync(login);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnForbidden_WhenUserIsInactive()
        {
            // Arrange
            var login = new UserLogin { Username = "inactive", UserPassword = "123" };
            var user = new User { UserId = Guid.NewGuid(), Username = login.Username, IsActive = false };
            _userGetterMock.Setup(u => u.GetUserByUsernameAsync(login.Username)).ReturnsAsync(user);

            // Act
            var result = await _service.LoginUserAsync(login);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.NotNull(result.Message);
            Assert.Equal(403, result.StatusCode);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnNotFound_WhenAuthNotFound()
        {
            // Arrange
            var login = new UserLogin { Username = "user", UserPassword = "123" };
            var user = new User { UserId = Guid.NewGuid(), Username = login.Username, IsActive = true };
            _userGetterMock.Setup(u => u.GetUserByUsernameAsync(login.Username)).ReturnsAsync(user);
            _authRepositoryMock.Setup(a => a.GetAuthByUserIdAsync(user.UserId)).ReturnsAsync((Auth?)null);

            // Act
            var result = await _service.LoginUserAsync(login);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.NotNull(result.Message);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnUnauthorized_WhenPasswordIncorrect()
        {
            // Arrange
            var login = new UserLogin { Username = "user", UserPassword = "wrong-pass" };
            var user = new User { UserId = Guid.NewGuid(), Username = login.Username, IsActive = true };
            _userGetterMock.Setup(u => u.GetUserByUsernameAsync(login.Username)).ReturnsAsync(user);

            var salt = new byte[16];
            RandomNumberGenerator.Fill(salt);
            var correctHash = _passwordHasher.HashPassword("correct-pass", salt);
            var auth = new Auth { UserId = user.UserId, PasswordSalt = salt, PasswordHash = correctHash };

            _authRepositoryMock.Setup(a => a.GetAuthByUserIdAsync(user.UserId)).ReturnsAsync(auth);

            // Act
            var result = await _service.LoginUserAsync(login);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.NotNull(result.Message);
            Assert.Equal(401, result.StatusCode);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnSuccess_WhenPasswordCorrect()
        {
            // Arrange
            var login = new UserLogin { Username = "user", UserPassword = "correct-pass" };
            var user = new User { UserId = Guid.NewGuid(), Username = login.Username, IsActive = true };
            _userGetterMock.Setup(u => u.GetUserByUsernameAsync(login.Username)).ReturnsAsync(user);

            var salt = new byte[16];
            RandomNumberGenerator.Fill(salt);
            var hash = _passwordHasher.HashPassword(login.UserPassword, salt);
            var auth = new Auth { UserId = user.UserId, PasswordSalt = salt, PasswordHash = hash };

            _authRepositoryMock.Setup(a => a.GetAuthByUserIdAsync(user.UserId)).ReturnsAsync(auth);

            // Act
            var result = await _service.LoginUserAsync(login);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Null(result.Message);
            Assert.Equal(0, result.StatusCode);
        }
    }
}
