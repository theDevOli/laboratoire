using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Services.UserServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.UserServices
{
    public class UserPatchServiceTest
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ILogger<UserPatchService>> _loggerMock;
        private readonly IUserPatchService _service;

        public UserPatchServiceTest()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UserPatchService>>();
            _service = new UserPatchService(_userRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task UpdateUserStatusAsync_ShouldReturnBadRequest_WhenUserIdIsNull()
        {
            // Act
            var result = await _service.UpdateUserStatusAsync(null);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(400, result.StatusCode);
            _userRepoMock.Verify(r => r.GetUserByIdAsync(It.IsAny<Guid?>()), Times.Never);
            _userRepoMock.Verify(r => r.UpdateUserStatusAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUserStatusAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepoMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _service.UpdateUserStatusAsync(userId);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
            _userRepoMock.Verify(r => r.GetUserByIdAsync(It.IsAny<Guid?>()), Times.Once);
            _userRepoMock.Verify(r => r.UpdateUserStatusAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUserStatusAsync_ShouldToggleIsActive_AndReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { UserId = userId, IsActive = true };
            _userRepoMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.UpdateUserStatusAsync(It.IsAny<User>())).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateUserStatusAsync(userId);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.True(user.IsActive == false);
            _userRepoMock.Verify(r => r.GetUserByIdAsync(It.IsAny<Guid?>()), Times.Once);
            _userRepoMock.Verify(r => r.UpdateUserStatusAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserStatusAsync_ShouldReturnDbError_WhenUpdateFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { UserId = userId, IsActive = true };
            _userRepoMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.UpdateUserStatusAsync(It.IsAny<User>())).ReturnsAsync(false);

            // Act
            var result = await _service.UpdateUserStatusAsync(userId);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            _userRepoMock.Verify(r => r.GetUserByIdAsync(It.IsAny<Guid?>()), Times.Once);
            _userRepoMock.Verify(r => r.UpdateUserStatusAsync(user), Times.Once);
        }
    }
}
