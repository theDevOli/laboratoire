using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.PermissionServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.PermissionServices;

public class PermissionAdderServiceTest
{
    private readonly Mock<IPermissionRepository> _repositoryMock;
    private readonly Mock<ILogger<PermissionAdderService>> _loggerMock;
    private readonly PermissionAdderService _service;

    public PermissionAdderServiceTest()
    {
        _repositoryMock = new Mock<IPermissionRepository>();
        _loggerMock = new Mock<ILogger<PermissionAdderService>>();
        _service = new PermissionAdderService(
            _repositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task AddPermissionAsync_ShouldReturnConflict_WhenPermissionAlreadyExists()
    {
        // Arrange
        var permissionDto = new PermissionDtoAdd { RoleId = 1 };

        _repositoryMock.Setup(r => r.DoesPermissionExistByRoleIdAsync(It.IsAny<Permission>()))
                       .ReturnsAsync(true);

        // Act
        var result = await _service.AddPermissionAsync(permissionDto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(409, result.StatusCode);
        Assert.Equal(ErrorMessage.ConflictPost, result.Message);
        _repositoryMock.Verify(r => r.AddPermissionAsync(It.IsAny<Permission>()), Times.Never);
    }

    [Fact]
    public async Task AddPermissionAsync_ShouldReturnDbError_WhenPermissionInsertionFails()
    {
        // Arrange
        var permissionDto = new PermissionDtoAdd { RoleId = 1 };

        _repositoryMock.Setup(r => r.DoesPermissionExistByRoleIdAsync(It.IsAny<Permission>()))
                       .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddPermissionAsync(It.IsAny<Permission>()))
                       .ReturnsAsync(false);

        // Act
        var result = await _service.AddPermissionAsync(permissionDto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(500, result.StatusCode);
        Assert.Equal(ErrorMessage.DbError, result.Message);
        _repositoryMock.Verify(r => r.AddPermissionAsync(It.IsAny<Permission>()), Times.Once);
    }

    [Fact]
    public async Task AddPermissionAsync_ShouldReturnSuccess_WhenPermissionAddedSuccessfully()
    {
        // Arrange
        var permissionDto = new PermissionDtoAdd { RoleId = 1 };

        _repositoryMock.Setup(r => r.DoesPermissionExistByRoleIdAsync(It.IsAny<Permission>()))
                       .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddPermissionAsync(It.IsAny<Permission>()))
                       .ReturnsAsync(true);

        // Act
        var result = await _service.AddPermissionAsync(permissionDto);

        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.Equal(0, result.StatusCode);
        Assert.Null(result.Message);
        _repositoryMock.Verify(r => r.AddPermissionAsync(It.IsAny<Permission>()), Times.Once);
    }
}