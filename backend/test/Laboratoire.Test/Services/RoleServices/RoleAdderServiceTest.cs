using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.RoleServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.RoleServices
{
    public class RoleAdderServiceTest
    {
        private readonly Mock<IRoleRepository> _roleRepoMock;
        private readonly Mock<ILogger<RoleAdderService>> _loggerMock;
        private readonly RoleAdderService _service;

        public RoleAdderServiceTest()
        {
            _roleRepoMock = new Mock<IRoleRepository>();
            _loggerMock = new Mock<ILogger<RoleAdderService>>();
            _service = new RoleAdderService(_roleRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddRoleAsync_ShouldReturnConflict_WhenRoleExists()
        {
            var dto = new RoleDtoAdd { RoleName = "Admin" };
            _roleRepoMock.Setup(r => r.DoesRoleExistByNameAsync(It.IsAny<Role>()))
                         .ReturnsAsync(true);

            var result = await _service.AddRoleAsync(dto);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(409, result.StatusCode);
            _roleRepoMock.Verify(r => r.DoesRoleExistByNameAsync(It.IsAny<Role>()), Times.Once);
            _roleRepoMock.Verify(r => r.AddRoleAsync(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task AddRoleAsync_ShouldReturnDbError_WhenAddFails()
        {
            var dto = new RoleDtoAdd { RoleName = "Admin" };
            _roleRepoMock.Setup(r => r.DoesRoleExistByNameAsync(It.IsAny<Role>()))
                         .ReturnsAsync(false);
            _roleRepoMock.Setup(r => r.AddRoleAsync(It.IsAny<Role>()))
                         .ReturnsAsync(false);

            var result = await _service.AddRoleAsync(dto);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            _roleRepoMock.Verify(r => r.DoesRoleExistByNameAsync(It.IsAny<Role>()), Times.Once);
            _roleRepoMock.Verify(r => r.AddRoleAsync(It.IsAny<Role>()), Times.Once);
        }

        [Fact]
        public async Task AddRoleAsync_ShouldReturnSuccess_WhenRoleAdded()
        {
            var dto = new RoleDtoAdd { RoleName = "Admin" };
            _roleRepoMock.Setup(r => r.DoesRoleExistByNameAsync(It.IsAny<Role>()))
                         .ReturnsAsync(false);
            _roleRepoMock.Setup(r => r.AddRoleAsync(It.IsAny<Role>()))
                         .ReturnsAsync(true);

            var result = await _service.AddRoleAsync(dto);

            Assert.False(result.IsNotSuccess());
            _roleRepoMock.Verify(r => r.DoesRoleExistByNameAsync(It.IsAny<Role>()), Times.Once);
            _roleRepoMock.Verify(r => r.AddRoleAsync(It.IsAny<Role>()), Times.Once);
        }
    }
}
