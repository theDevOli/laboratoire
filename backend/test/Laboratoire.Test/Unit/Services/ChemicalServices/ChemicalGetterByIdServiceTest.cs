using Laboratoire.Application.Services.ChemicalServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;
using Laboratoire.Application.ServicesContracts;

namespace Laboratoire.Test.Services.ChemicalServices;

public class ChemicalGetterByIdServiceTest
{
    private readonly Mock<IChemicalRepository> _chemicalRepositoryMock;
    private readonly Mock<IChemicalsNormalizationGetterService> _chemicalsNormalizationGetterServiceMock;
    private readonly Mock<ILogger<ChemicalGetterByIdService>> _loggerMock;
    private readonly ChemicalGetterByIdService _service;

    public ChemicalGetterByIdServiceTest()
    {
        _chemicalRepositoryMock = new Mock<IChemicalRepository>();
        _chemicalsNormalizationGetterServiceMock = new Mock<IChemicalsNormalizationGetterService>();
        _loggerMock = new Mock<ILogger<ChemicalGetterByIdService>>();

        _service = new ChemicalGetterByIdService(
            _chemicalRepositoryMock.Object,
            _chemicalsNormalizationGetterServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetChemicalByIdAsync_ShouldReturnNull_WhenIdIsNull()
    {
        // Act
        var result = await _service.GetChemicalByIdAsync(null);

        // Assert
        Assert.Null(result);
        _chemicalRepositoryMock.Verify(r => r.GetChemicalByIdAsync(It.IsAny<int>()), Times.Never);
        _chemicalsNormalizationGetterServiceMock.Verify(s => s.GetAllHazardsAsync(), Times.Never);
    }

    [Fact]
    public async Task GetChemicalByIdAsync_ShouldReturnChemicalDto_WhenChemicalExists()
    {
        // Arrange
        var chemicalId = 1;
        var chemical = new Chemical { ChemicalId = chemicalId, ChemicalName = "Water", Concentration = "100%" };

        var hazards = new ChemicalsNormalization[]
        {
            new ChemicalsNormalization { HazardId = 1 },
            new ChemicalsNormalization { HazardId = 2 }
        };

        _chemicalRepositoryMock
            .Setup(r => r.GetChemicalByIdAsync(chemicalId))
            .ReturnsAsync(chemical);

        _chemicalsNormalizationGetterServiceMock
            .Setup(s => s.GetAllHazardsAsync())
            .ReturnsAsync(hazards);

        // Act
        var result = await _service.GetChemicalByIdAsync(chemicalId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(chemicalId, result!.ChemicalId);
        Assert.Equal("Water", result.ChemicalName);
        Assert.Equal("100%", result.Concentration);

        _chemicalRepositoryMock.Verify(r => r.GetChemicalByIdAsync(chemicalId), Times.Once);
        _chemicalsNormalizationGetterServiceMock.Verify(s => s.GetAllHazardsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetChemicalByIdAsync_ShouldReturnNull_WhenChemicalDoesNotExist()
    {
        // Arrange
        var chemicalId = 1;
        var hazards = new List<ChemicalsNormalization>();

        _chemicalRepositoryMock
            .Setup(r => r.GetChemicalByIdAsync(chemicalId))
            .ReturnsAsync((Chemical?)null);

        _chemicalsNormalizationGetterServiceMock
            .Setup(s => s.GetAllHazardsAsync())
            .ReturnsAsync(hazards);

        // Act
        var result = await _service.GetChemicalByIdAsync(chemicalId);

        // Assert
        Assert.Null(result);

        _chemicalRepositoryMock.Verify(r => r.GetChemicalByIdAsync(chemicalId), Times.Once);
        _chemicalsNormalizationGetterServiceMock.Verify(s => s.GetAllHazardsAsync(), Times.Once);
    }
}
