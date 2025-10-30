using Laboratoire.Application.Services.OfficeServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Unit.Services.OfficeServices;

public class OfficeAdderServiceTest
{
    private readonly Mock<IOfficeRepository> _mockRepository;
    private readonly Mock<ILogger<OfficeAdderService>> _logger;
    private readonly OfficeAdderService _service;

    public OfficeAdderServiceTest()
    {
        _mockRepository = new();
        _logger = new();

        _service = new OfficeAdderService(_mockRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task AddOfficeAsync_ShouldReturnOk_WhenOperationSucceeds()
    {
        // Arrange
        var office = new Office() { OfficeId = Guid.NewGuid(), OfficeEmail = "test@email.com", OfficeName = "Test" };

        _mockRepository.Setup(repo => repo.DoesOfficeExistByCityAndNameAsync(It.IsAny<Office>())).ReturnsAsync(false);
        _mockRepository.Setup(repo => repo.AddOfficeAsync(It.IsAny<Office>())).ReturnsAsync(true);

        // Act
        var result = await _service.AddOfficeAsync(office);

        Assert.Null(result.Message);
        Assert.Equal(0, result.StatusCode);
        _mockRepository.Verify(repo => repo.DoesOfficeExistByCityAndNameAsync(It.IsAny<Office>()), Times.Once);
        _mockRepository.Verify(repo => repo.AddOfficeAsync(It.IsAny<Office>()), Times.Once);
    }

    [Fact]
    public async Task AddOfficeAsync_ShouldReturnConflict_WhenOfficeIsAlreadyOnDatabase()
    {
        // Arrange
        var office = new Office() { OfficeId = Guid.NewGuid(), OfficeEmail = "test@email.com", OfficeName = "Test" };

        _mockRepository.Setup(repo => repo.DoesOfficeExistByCityAndNameAsync(It.IsAny<Office>())).ReturnsAsync(true);

        // Act
        var result = await _service.AddOfficeAsync(office);

        Assert.True(result.IsNotSuccess());
        Assert.Equal(409, result.StatusCode);
        Assert.Equal(ErrorMessage.ConflictPost, result.Message);
        _mockRepository.Verify(repo => repo.DoesOfficeExistByCityAndNameAsync(It.IsAny<Office>()), Times.Once);
        _mockRepository.Verify(repo => repo.AddOfficeAsync(It.IsAny<Office>()), Times.Never);
    }

}
