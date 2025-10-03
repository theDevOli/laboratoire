using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.UserServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.ServicesContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.UserServices
{
    public class UserAdderServiceTest
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IAuthRegistrationService> _authRegMock;
        private readonly Mock<ILogger<UserAdderService>> _loggerMock;
        private readonly UserAdderService _service;

        public UserAdderServiceTest()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _authRegMock = new Mock<IAuthRegistrationService>();
            _loggerMock = new Mock<ILogger<UserAdderService>>();
            _service = new UserAdderService(_userRepoMock.Object, _authRegMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddUserAsync_ShouldSetUsername_WhenRoleIdIs4()
        {
            // Arrange
            var dto = new UserDtoAdd { Username = "originalUser", RoleId = 4 };
            var userId = Guid.NewGuid();
            _userRepoMock.Setup(r => r.SetUserNameAsync(dto.Username))
                         .ReturnsAsync("modifiedUser01");
            _userRepoMock.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                         .ReturnsAsync(userId);
            _authRegMock.Setup(a => a.RegisterUserAsync(It.IsAny<UserRegistration>()))
                        .ReturnsAsync(Error.SetSuccess());

            // Act
            var result = await _service.AddUserAsync(dto);

            // Assert
            Assert.Equal(userId, result);
            _userRepoMock.Verify(r => r.SetUserNameAsync(It.IsAny<string?>()), Times.Once);
            _userRepoMock.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Once);
            _authRegMock.Verify(a => a.RegisterUserAsync(It.IsAny<UserRegistration>()), Times.Once);
        }

        [Fact]
        public async Task AddUserAsync_ShouldReturnUserId_WhenRegistrationSucceeds()
        {
            // Arrange
            var dto = new UserDtoAdd { Username = "testUser", RoleId = 1 };
            var userId = Guid.NewGuid();
            _userRepoMock.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                         .ReturnsAsync(userId);
            _authRegMock.Setup(a => a.RegisterUserAsync(It.IsAny<UserRegistration>()))
                        .ReturnsAsync(Error.SetSuccess());

            // Act
            var result = await _service.AddUserAsync(dto);

            // Assert
            Assert.Equal(userId, result);
            _userRepoMock.Verify(r => r.SetUserNameAsync(It.IsAny<string?>()), Times.Never);
            _userRepoMock.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Once);
            _authRegMock.Verify(a => a.RegisterUserAsync(It.IsAny<UserRegistration>()), Times.Once);
        }

        [Fact]
        public async Task AddUserAsync_ShouldReturnNull_WhenRegistrationFails()
        {
            // Arrange
            var dto = new UserDtoAdd { Username = "testUser", RoleId = 1 };
            var userId = Guid.NewGuid();
            _userRepoMock.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                         .ReturnsAsync(userId);
            _authRegMock.Setup(a => a.RegisterUserAsync(It.IsAny<UserRegistration>()))
                        .ReturnsAsync(Error.SetError(ErrorMessage.DbError, 500));

            // Act
            var result = await _service.AddUserAsync(dto);

            // Assert
            Assert.Null(result);

            _userRepoMock.Verify(r => r.SetUserNameAsync(It.IsAny<string?>()), Times.Never);
            _userRepoMock.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Once);
            _authRegMock.Verify(r => r.RegisterUserAsync(It.IsAny<UserRegistration>()), Times.Once);
        }
    }
}
