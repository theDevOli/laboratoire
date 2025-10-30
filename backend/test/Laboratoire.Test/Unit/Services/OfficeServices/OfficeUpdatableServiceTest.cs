using Laboratoire.Application.Services.OfficeServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Unit.Services.OfficeServices;

public class OfficeUpdatableServiceTest
{
    private readonly Mock<IOfficeRepository> _mockRepository;
    private readonly Mock<ILogger<OfficeUpdatableService>> _logger;
    private readonly OfficeUpdatableService _service;
    public OfficeUpdatableServiceTest()
    {
        _mockRepository = new();
        _logger = new();

        _service = new OfficeUpdatableService(_mockRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task UpdateOfficeAsync_ShouldReturnOk_WhenOperationSucceeds()
    {
        // Arrange
        var office = new Office() { OfficeId = Guid.NewGuid(), OfficeEmail = "test@email.com", OfficeName = "Test" };

        _mockRepository.Setup(repo => repo.DoesOfficeExistByIdAsync(It.IsAny<Office>())).ReturnsAsync(true);
        _mockRepository.Setup(repo => repo.UpdateOfficeAsync(It.IsAny<Office>())).ReturnsAsync(true);

        // Act
        var result = await _service.UpdateOfficeAsync(office);

        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.Null(result.Message);
        Assert.Equal(0, result.StatusCode);
        _mockRepository.Verify(repo => repo.DoesOfficeExistByIdAsync(It.IsAny<Office>()), Times.Once);
        _mockRepository.Verify(repo => repo.UpdateOfficeAsync(It.IsAny<Office>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOfficeAsync_ShouldReturnNotFound_WhenThereIsNoOfficeOnDatabase()
    {
        // Arrange
        var office = new Office() { OfficeId = Guid.NewGuid(), OfficeEmail = "test@email.com", OfficeName = "Test" };

        _mockRepository.Setup(repo => repo.DoesOfficeExistByIdAsync(It.IsAny<Office>())).ReturnsAsync(false);

        // Act
        var result = await _service.UpdateOfficeAsync(office);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(404, result.StatusCode);
        Assert.Equal(ErrorMessage.NotFound, result.Message);
        _mockRepository.Verify(repo => repo.DoesOfficeExistByIdAsync(It.IsAny<Office>()), Times.Once);
        _mockRepository.Verify(repo => repo.UpdateOfficeAsync(It.IsAny<Office>()), Times.Never);
    }
}
