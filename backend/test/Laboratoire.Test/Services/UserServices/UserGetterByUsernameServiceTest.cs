using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Services.UserServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.UserServices
{
    public class UserGetterByUsernameServiceTest
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ILogger<UserGetterByUsernameService>> _loggerMock;
        private readonly IUserGetterByUsernameService _service;

        public UserGetterByUsernameServiceTest()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UserGetterByUsernameService>>();
            _service = new UserGetterByUsernameService(_userRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldReturnNull_WhenUsernameIsNull()
        {
            // Arrange
            string? username = null;

            // Act
            var result = await _service.GetUserByUsernameAsync(username);

            // Assert
            Assert.Null(result);
            _userRepoMock.Verify(r => r.GetUserByUsernameAsync(username), Times.Never);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldReturnUser_WhenUsernameExists()
        {
            // Arrange
            string username = "john.doe";
            var user = new User { UserId = Guid.NewGuid(), Username = username };
            _userRepoMock.Setup(r => r.GetUserByUsernameAsync(username)).ReturnsAsync(user);

            // Act
            var result = await _service.GetUserByUsernameAsync(username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result!.Username);
            _userRepoMock.Verify(r => r.GetUserByUsernameAsync(username), Times.Once);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            string username = "nonexistent.user";
            _userRepoMock.Setup(r => r.GetUserByUsernameAsync(username)).ReturnsAsync((User?)null);

            // Act
            var result = await _service.GetUserByUsernameAsync(username);

            // Assert
            Assert.Null(result);
            _userRepoMock.Verify(r => r.GetUserByUsernameAsync(username), Times.Once);
        }
    }
}
