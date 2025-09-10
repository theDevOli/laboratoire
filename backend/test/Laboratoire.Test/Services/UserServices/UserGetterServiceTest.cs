using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Services.UserServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.UserServices
{
    public class UserGetterServiceTest
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ILogger<UserGetterService>> _loggerMock;
        private readonly IUserGetterService _service;

        public UserGetterServiceTest()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UserGetterService>>();
            _service = new UserGetterService(_userRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnUsers_WhenUsersExist()
        {
            // Arrange
            var users = new List<DisplayUser>
            {
                new DisplayUser { UserId = Guid.NewGuid(), Username = "john.doe" },
                new DisplayUser { UserId = Guid.NewGuid(), Username = "jane.doe" }
            };

            _userRepoMock.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Collection
                (
                    users,
                    item => Assert.Equal(item.UserId,users[0].UserId),
                    item => Assert.Equal(item.UserId,users[1].UserId)
                );
            _userRepoMock.Verify(r => r.GetAllUsersAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var emptyList = Enumerable.Empty<DisplayUser>();
            _userRepoMock.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(emptyList);

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _userRepoMock.Verify(r => r.GetAllUsersAsync(), Times.Once);
        }
    }
}
