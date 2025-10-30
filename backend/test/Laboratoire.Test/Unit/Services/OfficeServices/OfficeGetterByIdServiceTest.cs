using Laboratoire.Application.Services.OfficeServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Unit.Services.OfficeServices;

public class OfficeGetterByIdServiceTest
{
    private readonly Mock<IOfficeRepository> _mockRepository;
    private readonly Mock<ILogger<OfficeGetterByIdService>> _mockLogger;
    private readonly OfficeGetterByIdService _service;

    public OfficeGetterByIdServiceTest()
    {
        _mockRepository = new();
        _mockLogger = new();

        _service = new OfficeGetterByIdService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetByOfficeIdAsync_ShouldReturnNull_WhenOfficeIdIsNull()
    {
        // Arrange
        Guid? officeId = null;
        _mockRepository.Setup(repo => repo.GetOfficeByIdAsync(officeId)).ReturnsAsync((Office?)null);

        // Act
        var result = await _service.GetByOfficeIdAsync(officeId);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(repo => repo.GetOfficeByIdAsync(It.IsAny<Guid?>()), Times.Never);
    }

    [Fact]
    public async Task GetByOfficeIdAsync_ShouldReturnOffice_WhenOfficeExists()
    {
        // Arrange
        Guid officeId = Guid.NewGuid();
        var office = new Office()
        {
            OfficeId = officeId,
            OfficeEmail = "test@email.com",
            OfficeName = "Test"
        };
        _mockRepository.Setup(repo => repo.GetOfficeByIdAsync(officeId)).ReturnsAsync(office);

        // Act
        var result = await _service.GetByOfficeIdAsync(officeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.OfficeId,officeId);
        _mockRepository.Verify(repo => repo.GetOfficeByIdAsync(It.IsAny<Guid?>()), Times.Once);
    }
}
