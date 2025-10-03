using Laboratoire.Application.Services.PermissionServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.PermissionServices;
    public class PermissionUpdatableServiceTest
    {
        private readonly Mock<IPermissionRepository> _repositoryMock;
        private readonly Mock<ILogger<PermissionUpdatableService>> _loggerMock;
        private readonly PermissionUpdatableService _service;

        public PermissionUpdatableServiceTest()
        {
            _repositoryMock = new Mock<IPermissionRepository>();
            _loggerMock = new Mock<ILogger<PermissionUpdatableService>>();
            _service = new PermissionUpdatableService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task UpdatePermissionAsync_ShouldReturnNotFound_WhenPermissionDoesNotExist()
        {
            // Arrange
            var permission = new Permission { PermissionId = 1 };

            _repositoryMock.Setup(r => r.DoesPermissionExistByPermissionIdAsync(It.IsAny<Permission>()))
                           .ReturnsAsync(false);

            // Act
            var result = await _service.UpdatePermissionAsync(permission);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
            Assert.Equal(ErrorMessage.NotFound, result.Message);
            _repositoryMock.Verify(r => r.UpdatePermissionAsync(It.IsAny<Permission>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePermissionAsync_ShouldReturnDbError_WhenPermissionUpdateFails()
        {
            // Arrange
            var permission = new Permission { PermissionId = 1};

            _repositoryMock.Setup(r => r.DoesPermissionExistByPermissionIdAsync(It.IsAny<Permission>()))
                           .ReturnsAsync(true);
            _repositoryMock.Setup(r => r.UpdatePermissionAsync(It.IsAny<Permission>()))
                           .ReturnsAsync(false);

            // Act
            var result = await _service.UpdatePermissionAsync(permission);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _repositoryMock.Verify(r => r.UpdatePermissionAsync(It.IsAny<Permission>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePermissionAsync_ShouldReturnSuccess_WhenPermissionUpdatedSuccessfully()
        {
            // Arrange
            var permission = new Permission { PermissionId = 1 };

            _repositoryMock.Setup(r => r.DoesPermissionExistByPermissionIdAsync(It.IsAny<Permission>()))
                           .ReturnsAsync(true);
            _repositoryMock.Setup(r => r.UpdatePermissionAsync(It.IsAny<Permission>()))
                           .ReturnsAsync(true);

            // Act
            var result = await _service.UpdatePermissionAsync(permission);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null(result.Message);
            _repositoryMock.Verify(r => r.UpdatePermissionAsync(It.IsAny<Permission>()), Times.Once);
        }
    }