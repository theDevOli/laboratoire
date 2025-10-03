using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Services.UserServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.UserServices
{
    public class UserRenameServiceTest
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ILogger<UserRenameService>> _loggerMock;
        private readonly IUserRenameService _service;

        public UserRenameServiceTest()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UserRenameService>>();
            _service = new UserRenameService(_userRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task UserRenameAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userDto = new UserDtoRename { UserId = Guid.NewGuid(), Username = "newName" };
            _userRepoMock.Setup(r => r.DoesUserExistByIdAsync(It.IsAny<User>())).ReturnsAsync(false);

            // Act
            var result = await _service.UserRenameAsync(userDto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
            _userRepoMock.Verify(r => r.DoesUserExistByIdAsync(It.Is<User>(u => u.UserId == userDto.UserId)), Times.Once);
            _userRepoMock.Verify(r => r.UserRenameAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UserRenameAsync_ShouldReturnDbError_WhenRenameFails()
        {
            // Arrange
            var userDto = new UserDtoRename { UserId = Guid.NewGuid(), Username = "newName" };
            _userRepoMock.Setup(r => r.DoesUserExistByIdAsync(It.IsAny<User>())).ReturnsAsync(true);
            _userRepoMock.Setup(r => r.UserRenameAsync(It.IsAny<User>())).ReturnsAsync(false);

            // Act
            var result = await _service.UserRenameAsync(userDto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            _userRepoMock.Verify(r => r.DoesUserExistByIdAsync(It.Is<User>(u => u.UserId == userDto.UserId)), Times.Once);
            _userRepoMock.Verify(r => r.UserRenameAsync(It.Is<User>(u => u.UserId == userDto.UserId)), Times.Once);
        }

        [Fact]
        public async Task UserRenameAsync_ShouldReturnSuccess_WhenRenameSucceeds()
        {
            // Arrange
            var userDto = new UserDtoRename { UserId = Guid.NewGuid(), Username = "newName" };
            _userRepoMock.Setup(r => r.DoesUserExistByIdAsync(It.IsAny<User>())).ReturnsAsync(true);
            _userRepoMock.Setup(r => r.UserRenameAsync(It.IsAny<User>())).ReturnsAsync(true);

            // Act
            var result = await _service.UserRenameAsync(userDto);

            // Assert
            Assert.False(result.IsNotSuccess());
            _userRepoMock.Verify(r => r.DoesUserExistByIdAsync(It.Is<User>(u => u.UserId == userDto.UserId)), Times.Once);
            _userRepoMock.Verify(r => r.UserRenameAsync(It.IsAny<User>()), Times.Once);
        }
    }
}
