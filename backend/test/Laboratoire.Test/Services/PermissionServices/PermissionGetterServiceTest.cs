using Laboratoire.Application.Services.PermissionServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.PermissionServices;

public class PermissionGetterServiceTest
{
    private readonly Mock<IPermissionRepository> _repositoryMock;
    private readonly Mock<ILogger<PermissionGetterService>> _loggerMock;
    private readonly PermissionGetterService _service;

    public PermissionGetterServiceTest()
    {
        _repositoryMock = new Mock<IPermissionRepository>();
        _loggerMock = new Mock<ILogger<PermissionGetterService>>();
        _service = new PermissionGetterService(
            _repositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetAllPermissionsAsync_ShouldReturnPermissions_WhenPermissionExists()
    {
        // Arrange
        var expectedPermissions = new List<DisplayPermission>
            {
                new DisplayPermission { PermissionId = 1, },
                new DisplayPermission { PermissionId = 2, }
            };

        _repositoryMock.Setup(r => r.GetAllPermissionsAsync())
                       .ReturnsAsync(expectedPermissions);

        // Act
        var result = await _service.GetAllPermissionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Collection
        (
            expectedPermissions,
            item => Assert.Equal(item.PermissionId, expectedPermissions[0].PermissionId),
            item => Assert.Equal(item.PermissionId, expectedPermissions[1].PermissionId)
        );
        _repositoryMock.Verify(r => r.GetAllPermissionsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllPermissionsAsync_ShouldReturnEmptyList_WhenNoPermissionExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllPermissionsAsync())
                       .ReturnsAsync(Enumerable.Empty<DisplayPermission>());

        // Act
        var result = await _service.GetAllPermissionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _repositoryMock.Verify(r => r.GetAllPermissionsAsync(), Times.Once);
    }
}