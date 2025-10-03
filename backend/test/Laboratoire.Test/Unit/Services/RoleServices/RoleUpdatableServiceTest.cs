using Laboratoire.Application.Services.RoleServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.RoleServices
{
    public class RoleUpdatableServiceTest
    {
        private readonly Mock<IRoleRepository> _roleRepoMock;
        private readonly Mock<ILogger<RoleUpdatableService>> _loggerMock;
        private readonly RoleUpdatableService _service;

        public RoleUpdatableServiceTest()
        {
            _roleRepoMock = new Mock<IRoleRepository>();
            _loggerMock = new Mock<ILogger<RoleUpdatableService>>();
            _service = new RoleUpdatableService(_roleRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task UpdateRoleAsync_ShouldReturnNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var role = new Role { RoleId = 1, RoleName = "Admin" };
            _roleRepoMock.Setup(r => r.DoesRoleExistByIdAsync(role)).ReturnsAsync(false);

            // Act
            var result = await _service.UpdateRoleAsync(role);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
            _roleRepoMock.Verify(r => r.DoesRoleExistByIdAsync(It.IsAny<Role>()), Times.Once);
            _roleRepoMock.Verify(r => r.DoesRoleExistByNameAsync(It.IsAny<Role>()), Times.Never);
            _roleRepoMock.Verify(r => r.UpdateRoleAsync(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task UpdateRoleAsync_ShouldReturnConflict_WhenRoleNameAlreadyExists()
        {
            // Arrange
            var role = new Role { RoleId = 1, RoleName = "Admin" };
            _roleRepoMock.Setup(r => r.DoesRoleExistByIdAsync(role)).ReturnsAsync(true);
            _roleRepoMock.Setup(r => r.DoesRoleExistByNameAsync(role)).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateRoleAsync(role);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(409, result.StatusCode);
            _roleRepoMock.Verify(r => r.DoesRoleExistByIdAsync(It.IsAny<Role>()), Times.Once);
            _roleRepoMock.Verify(r => r.DoesRoleExistByNameAsync(It.IsAny<Role>()), Times.Once);
            _roleRepoMock.Verify(r => r.UpdateRoleAsync(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task UpdateRoleAsync_ShouldReturnDbError_WhenUpdateFails()
        {
            // Arrange
            var role = new Role { RoleId = 1, RoleName = "Admin" };
            _roleRepoMock.Setup(r => r.DoesRoleExistByIdAsync(role)).ReturnsAsync(true);
            _roleRepoMock.Setup(r => r.DoesRoleExistByNameAsync(role)).ReturnsAsync(false);
            _roleRepoMock.Setup(r => r.UpdateRoleAsync(role)).ReturnsAsync(false);

            // Act
            var result = await _service.UpdateRoleAsync(role);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            _roleRepoMock.Verify(r => r.DoesRoleExistByIdAsync(It.IsAny<Role>()), Times.Once);
            _roleRepoMock.Verify(r => r.DoesRoleExistByNameAsync(It.IsAny<Role>()), Times.Once);
            _roleRepoMock.Verify(r => r.UpdateRoleAsync(It.IsAny<Role>()), Times.Once);
        }

        [Fact]
        public async Task UpdateRoleAsync_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            // Arrange
            var role = new Role { RoleId = 1, RoleName = "Admin" };
            _roleRepoMock.Setup(r => r.DoesRoleExistByIdAsync(role)).ReturnsAsync(true);
            _roleRepoMock.Setup(r => r.DoesRoleExistByNameAsync(role)).ReturnsAsync(false);
            _roleRepoMock.Setup(r => r.UpdateRoleAsync(role)).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateRoleAsync(role);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            _roleRepoMock.Verify(r => r.DoesRoleExistByIdAsync(It.IsAny<Role>()), Times.Once);
            _roleRepoMock.Verify(r => r.DoesRoleExistByNameAsync(It.IsAny<Role>()), Times.Once);
            _roleRepoMock.Verify(r => r.UpdateRoleAsync(It.IsAny<Role>()), Times.Once);
        }
    }
}
