using Laboratoire.Application.Services.UserServices;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.UserServices
{
    public class UserUpdatableServiceTest
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ILogger<UserUpdatableService>> _loggerMock;
        private readonly IUserUpdatableService _service;

        public UserUpdatableServiceTest()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UserUpdatableService>>();
            _service = new UserUpdatableService(_userRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var user = new User { UserId = Guid.NewGuid() };
            _userRepoMock.Setup(r => r.DoesUserExistByIdAsync(user)).ReturnsAsync(false);

            // Act
            var result = await _service.UpdateUserAsync(user);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
            _userRepoMock.Verify(r => r.DoesUserExistByIdAsync(It.IsAny<User>()), Times.Once);
            _userRepoMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldReturnDbError_WhenUpdateFails()
        {
            // Arrange
            var user = new User { UserId = Guid.NewGuid() };
            _userRepoMock.Setup(r => r.DoesUserExistByIdAsync(user)).ReturnsAsync(true);
            _userRepoMock.Setup(r => r.UpdateUserAsync(user)).ReturnsAsync(false);

            // Act
            var result = await _service.UpdateUserAsync(user);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            _userRepoMock.Verify(r => r.DoesUserExistByIdAsync(It.IsAny<User>()), Times.Once);
            _userRepoMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            // Arrange
            var user = new User { UserId = Guid.NewGuid() };
            _userRepoMock.Setup(r => r.DoesUserExistByIdAsync(user)).ReturnsAsync(true);
            _userRepoMock.Setup(r => r.UpdateUserAsync(user)).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateUserAsync(user);

            // Assert
            Assert.False(result.IsNotSuccess());
            _userRepoMock.Verify(r => r.DoesUserExistByIdAsync(It.IsAny<User>()), Times.Once);
            _userRepoMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Once);
        }
    }
}
