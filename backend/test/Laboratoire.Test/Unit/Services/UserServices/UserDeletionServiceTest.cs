using Laboratoire.Application.Services.UserServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.UserServices
{
    public class UserDeletionServiceTest
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ILogger<UserDeletionService>> _loggerMock;
        private readonly UserDeletionService _service;

        public UserDeletionServiceTest()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UserDeletionService>>();
            _service = new UserDeletionService(_userRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task DeletionUserAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var user = new User { UserId = Guid.NewGuid() };
            _userRepoMock.Setup(r => r.DoesUserExistByIdAsync(user)).ReturnsAsync(false);

            // Act
            var result = await _service.DeletionUserAsync(user);

            // Assert
            Assert.Equal(404, result.StatusCode);
            Assert.True(result.IsNotSuccess());
            _userRepoMock.Verify(r => r.DoesUserExistByIdAsync(It.IsAny<User>()), Times.Once);
            _userRepoMock.Verify(r => r.DeleteUserAsync(It.IsAny<Guid?>()), Times.Never);
        }

        [Fact]
        public async Task DeletionUserAsync_ShouldReturnDbError_WhenDeletionFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            User user = new() { UserId = userId };
            _userRepoMock.Setup(r => r.DoesUserExistByIdAsync(user)).ReturnsAsync(true);
            _userRepoMock.Setup(r => r.DeleteUserAsync(userId)).ReturnsAsync(false);

            // Act
            var result = await _service.DeletionUserAsync(user);

            // Assert
            Assert.Equal(500, result.StatusCode);
            Assert.True(result.IsNotSuccess());
            _userRepoMock.Verify(r => r.DoesUserExistByIdAsync(It.IsAny<User>()), Times.Once);
            _userRepoMock.Verify(r => r.DeleteUserAsync(It.IsAny<Guid?>()), Times.Once);
        }

        [Fact]
        public async Task DeletionUserAsync_ShouldReturnSuccess_WhenUserIsDeleted()
        {
            // Arrange
            var userId = Guid.NewGuid();
            User user = new() { UserId = userId };

            _userRepoMock.Setup(r => r.DoesUserExistByIdAsync(user)).ReturnsAsync(true);
            _userRepoMock.Setup(r => r.DeleteUserAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _service.DeletionUserAsync(user);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
        }
    }
}
