using Laboratoire.Application.Services.RoleServices;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.RoleServices
{
    public class RoleGetterByUserIdServiceTest
    {
        private readonly Mock<IRoleRepository> _roleRepoMock;
        private readonly Mock<ILogger<RoleGetterByUserIdService>> _loggerMock;
        private readonly RoleGetterByUserIdService _service;

        public RoleGetterByUserIdServiceTest()
        {
            _roleRepoMock = new Mock<IRoleRepository>();
            _loggerMock = new Mock<ILogger<RoleGetterByUserIdService>>();
            _service = new RoleGetterByUserIdService(_roleRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetRoleNameByUserIdAsync_ShouldReturnNull_WhenUserIdIsNull()
        {
            var result = await _service.GetRoleNameByUserIdAsync(null);

            Assert.Null(result);
            _roleRepoMock.Verify(r => r.GetRoleNameByUserIdAsync(It.IsAny<Guid?>()), Times.Once);
        }

        [Fact]
        public async Task GetRoleNameByUserIdAsync_ShouldReturnRoleName_WhenRoleExists()
        {
            var userId = Guid.NewGuid();
            _roleRepoMock.Setup(r => r.GetRoleNameByUserIdAsync(userId)).ReturnsAsync("Admin");

            var result = await _service.GetRoleNameByUserIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal("Admin", result);
            _roleRepoMock.Verify(r => r.GetRoleNameByUserIdAsync(It.IsAny<Guid?>()), Times.Once);
        }

        [Fact]
        public async Task GetRoleNameByUserIdAsync_ShouldReturnNull_WhenRoleDoesNotExist()
        {
            var userId = Guid.NewGuid();
            _roleRepoMock.Setup(r => r.GetRoleNameByUserIdAsync(userId)).ReturnsAsync((string?)null);

            var result = await _service.GetRoleNameByUserIdAsync(userId);

            Assert.Null(result);
            _roleRepoMock.Verify(r => r.GetRoleNameByUserIdAsync(It.IsAny<Guid?>()), Times.Once);
        }
    }
}
