using Laboratoire.Application.Services.CatalogServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CatalogServices;

public class CatalogUpdatableServiceTest
{
    private readonly Mock<ICatalogRepository> _catalogRepositoryMock;
    private readonly Mock<ILogger<CatalogUpdatableService>> _loggerMock;
    private readonly CatalogUpdatableService _service;

    public CatalogUpdatableServiceTest()
    {
        _catalogRepositoryMock = new Mock<ICatalogRepository>();
        _loggerMock = new Mock<ILogger<CatalogUpdatableService>>();

        _service = new CatalogUpdatableService(
            _catalogRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task UpdateCatalogAsync_ShouldReturnSuccess_WhenCatalogUpdated()
    {
        // Arrange
        var catalog = new Catalog { CatalogId = 1 };

        _catalogRepositoryMock.Setup(r => r.DoesCatalogExistByIdAsync(catalog)).ReturnsAsync(true);
        _catalogRepositoryMock.Setup(r => r.UpdateCatalogAsync(catalog)).ReturnsAsync(true);

        // Act
        var result = await _service.UpdateCatalogAsync(catalog);

        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.Null(result.Message);
        Assert.Equal(0, result.StatusCode);

        _catalogRepositoryMock.Verify(r => r.DoesCatalogExistByIdAsync(catalog), Times.Once);
        _catalogRepositoryMock.Verify(r => r.UpdateCatalogAsync(catalog), Times.Once);
    }

    [Fact]
    public async Task UpdateCatalogAsync_ShouldReturnNotFound_WhenCatalogDoesNotExist()
    {
        // Arrange
        var catalog = new Catalog { CatalogId = 1 };
        _catalogRepositoryMock.Setup(r => r.DoesCatalogExistByIdAsync(catalog)).ReturnsAsync(false);

        // Act
        var result = await _service.UpdateCatalogAsync(catalog);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(ErrorMessage.NotFound, result.Message);
        Assert.Equal(404, result.StatusCode);

        _catalogRepositoryMock.Verify(r => r.DoesCatalogExistByIdAsync(catalog), Times.Once);
        _catalogRepositoryMock.Verify(r => r.UpdateCatalogAsync(It.IsAny<Catalog>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCatalogAsync_ShouldReturnDbError_WhenUpdateFails()
    {
        // Arrange
        var catalog = new Catalog { CatalogId = 1 };

        _catalogRepositoryMock.Setup(r => r.DoesCatalogExistByIdAsync(catalog)).ReturnsAsync(true);
        _catalogRepositoryMock.Setup(r => r.UpdateCatalogAsync(catalog)).ReturnsAsync(false);

        // Act
        var result = await _service.UpdateCatalogAsync(catalog);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(ErrorMessage.DbError, result.Message);
        Assert.Equal(500, result.StatusCode);

        _catalogRepositoryMock.Verify(r => r.DoesCatalogExistByIdAsync(catalog), Times.Once);
        _catalogRepositoryMock.Verify(r => r.UpdateCatalogAsync(catalog), Times.Once);
    }
}
