using Laboratoire.Application.Services.RoleServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.RoleServices
{
    public class RoleGetterServiceTest
    {
        private readonly Mock<IRoleRepository> _roleRepoMock;
        private readonly Mock<ILogger<RoleGetterService>> _loggerMock;
        private readonly RoleGetterService _service;

        public RoleGetterServiceTest()
        {
            _roleRepoMock = new Mock<IRoleRepository>();
            _loggerMock = new Mock<ILogger<RoleGetterService>>();
            _service = new RoleGetterService(_roleRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllRolesAsync_ShouldReturnAllRoles()
        {
            // Arrange
            var roles = new List<Role>
            {
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "User" }
            };
            _roleRepoMock.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(roles);

            // Act
            var result = await _service.GetAllRolesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Collection
            (
                result,
                item => Assert.Equal(item.RoleName, roles[0].RoleName),
                item => Assert.Equal(item.RoleName, roles[1].RoleName)
            );
            _roleRepoMock.Verify(r => r.GetAllRolesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllRolesAsync_ShouldReturnEmpty_WhenNoRolesExist()
        {
            // Arrange
            _roleRepoMock.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(Enumerable.Empty<Role>());

            // Act
            var result = await _service.GetAllRolesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _roleRepoMock.Verify(r => r.GetAllRolesAsync(), Times.Once);
        }
    }
}
