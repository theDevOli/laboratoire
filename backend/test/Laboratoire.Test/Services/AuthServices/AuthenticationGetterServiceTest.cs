using Laboratoire.Application.Services.AuthServices;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Tests.Services.AuthServices
{
    public class AuthenticationGetterServiceTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<AuthenticationGetterService>> _loggerMock;
        private readonly IAuthenticationGetterService _service;

        public AuthenticationGetterServiceTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<AuthenticationGetterService>>();

            _service = new AuthenticationGetterService(
                _userRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAuthenticationByUserId_ShouldReturnAuthentication_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedAuth = new Authentication
            {
                Username = "testuser",
                Name = "Test User",
                IsActive = true
            };

            _userRepositoryMock
                .Setup(repo => repo.GetAuthenticationByIdAsync(userId))
                .ReturnsAsync(expectedAuth);

            // Act
            var result = await _service.GetAuthenticationByUserId(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedAuth, result);
            _userRepositoryMock.Verify(repo => repo.GetAuthenticationByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetAuthenticationByUserId_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(repo => repo.GetAuthenticationByIdAsync(userId))
                .ReturnsAsync((Authentication?)null);

            // Act
            var result = await _service.GetAuthenticationByUserId(userId);

            // Assert
            Assert.Null(result);
            _userRepositoryMock.Verify(repo => repo.GetAuthenticationByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetAuthenticationByUserId_ShouldLogInformation_WhenCalled()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock
                .Setup(repo => repo.GetAuthenticationByIdAsync(userId))
                .ReturnsAsync((Authentication?)null);

            // Act
            await _service.GetAuthenticationByUserId(userId);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(userId.ToString())),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }
    }
}
