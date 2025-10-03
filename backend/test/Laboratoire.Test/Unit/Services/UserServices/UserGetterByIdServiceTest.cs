using Laboratoire.Application.Services.UserServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.UserServices
{
    public class UserGetterByIdServiceTest
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ILogger<UserGetterByIdService>> _loggerMock;
        private readonly UserGetterByIdService _service;

        public UserGetterByIdServiceTest()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UserGetterByIdService>>();
            _service = new UserGetterByIdService(_userRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserIdIsNull()
        {
            // Act
            var result = await _service.GetUserByIdAsync(null);

            // Assert
            Assert.Null(result);
            _userRepoMock.Verify(r => r.GetUserByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { UserId = userId, Username = "testuser" };
            _userRepoMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result!.UserId);
            Assert.Equal("testuser", result.Username);
            _userRepoMock.Verify(r => r.GetUserByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenNoUserExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepoMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
            _userRepoMock.Verify(r => r.GetUserByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
