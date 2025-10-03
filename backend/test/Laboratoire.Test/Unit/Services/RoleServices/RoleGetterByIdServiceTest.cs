using Laboratoire.Application.Services.RoleServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.RoleServices
{
    public class RoleGetterByIdServiceTest
    {
        private readonly Mock<IRoleRepository> _roleRepoMock;
        private readonly Mock<ILogger<RoleGetterByIdService>> _loggerMock;
        private readonly RoleGetterByIdService _service;

        public RoleGetterByIdServiceTest()
        {
            _roleRepoMock = new Mock<IRoleRepository>();
            _loggerMock = new Mock<ILogger<RoleGetterByIdService>>();
            _service = new RoleGetterByIdService(_roleRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetRoleByIdAsync_ShouldReturnNull_WhenRoleIdIsNull()
        {
            var result = await _service.GetRoleByIdAsync(null);

            Assert.Null(result);
            _roleRepoMock.Verify(r => r.GetRoleByIdAsync(It.IsAny<int?>()), Times.Never);
        }

        [Fact]
        public async Task GetRoleByIdAsync_ShouldReturnRole_WhenRoleExists()
        {
            var roleId = 1;
            var role = new Role { RoleId = roleId, RoleName = "Admin" };
            _roleRepoMock.Setup(r => r.GetRoleByIdAsync(roleId)).ReturnsAsync(role);

            var result = await _service.GetRoleByIdAsync(roleId);

            Assert.NotNull(result);
            Assert.Equal(roleId, result.RoleId);
            Assert.Equal("Admin", result.RoleName);
            _roleRepoMock.Verify(r => r.GetRoleByIdAsync(It.IsAny<int?>()), Times.Once);
        }

        [Fact]
        public async Task GetRoleByIdAsync_ShouldReturnNull_WhenRoleDoesNotExist()
        {
            _roleRepoMock.Setup(r => r.GetRoleByIdAsync(2)).ReturnsAsync((Role?)null);

            var result = await _service.GetRoleByIdAsync(2);

            Assert.Null(result);
            _roleRepoMock.Verify(r => r.GetRoleByIdAsync(It.IsAny<int?>()), Times.Once);
        }
    }
}
