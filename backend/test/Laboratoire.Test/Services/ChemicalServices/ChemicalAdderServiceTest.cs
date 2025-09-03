using Laboratoire.Application.Services.ChemicalServices;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ChemicalServices;

public class ChemicalAdderServiceTest
{
    private readonly Mock<IChemicalRepository> _chemicalRepositoryMock;
    private readonly Mock<IChemicalsNormalizationAdderService> _chemicalsNormalizationAdderServiceMock;
    private readonly Mock<ILogger<ChemicalAdderService>> _loggerMock;
    private readonly ChemicalAdderService _service;

    public ChemicalAdderServiceTest()
    {
        _chemicalRepositoryMock = new Mock<IChemicalRepository>();
        _chemicalsNormalizationAdderServiceMock = new Mock<IChemicalsNormalizationAdderService>();
        _loggerMock = new Mock<ILogger<ChemicalAdderService>>();

        _service = new ChemicalAdderService(
            _chemicalRepositoryMock.Object,
            _chemicalsNormalizationAdderServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task AddChemicalAsync_ShouldReturnSuccess_WhenChemicalAddedAndHazardsAdded()
    {
        // Arrange
        var chemicalDto = new ChemicalDtoAdd
        {
            ChemicalName = "Water",
            Concentration = "100%",
            Hazards = [1, 2]
        };

        _chemicalRepositoryMock
            .Setup(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()))
            .ReturnsAsync(false);

        _chemicalRepositoryMock
            .Setup(r => r.AddChemicalAsync(It.IsAny<Chemical>()))
            .ReturnsAsync(1);

        _chemicalsNormalizationAdderServiceMock
            .Setup(s => s.AddHazardAsync(It.IsAny<IEnumerable<ChemicalsNormalization>>()))
            .ReturnsAsync(Error.SetSuccess());

        // Act
        var result = await _service.AddChemicalAsync(chemicalDto);

        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.Null(result.Message);
        Assert.Equal(0, result.StatusCode);

        _chemicalRepositoryMock.Verify(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()), Times.Once);
        _chemicalRepositoryMock.Verify(r => r.AddChemicalAsync(It.IsAny<Chemical>()), Times.Once);
        _chemicalsNormalizationAdderServiceMock.Verify(s => s.AddHazardAsync(It.IsAny<IEnumerable<ChemicalsNormalization>>()), Times.Once);
    }

    [Fact]
    public async Task AddChemicalAsync_ShouldReturnConflict_WhenChemicalAlreadyExists()
    {
        // Arrange
        var chemicalDto = new ChemicalDtoAdd
        {
            ChemicalName = "Water",
            Concentration = "100%"
        };

        _chemicalRepositoryMock
            .Setup(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.AddChemicalAsync(chemicalDto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(ErrorMessage.ConflictPost, result.Message);
        Assert.Equal(409, result.StatusCode);

        _chemicalRepositoryMock.Verify(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()), Times.Once);
        _chemicalRepositoryMock.Verify(r => r.AddChemicalAsync(It.IsAny<Chemical>()), Times.Never);
        _chemicalsNormalizationAdderServiceMock.Verify(s => s.AddHazardAsync(It.IsAny<IEnumerable<ChemicalsNormalization>>()), Times.Never);
    }

    [Fact]
    public async Task AddChemicalAsync_ShouldReturnDbError_WhenAddChemicalFails()
    {
        // Arrange
        var chemicalDto = new ChemicalDtoAdd
        {
            ChemicalName = "Water",
            Concentration = "100%",
            Hazards = [1]
        };

        _chemicalRepositoryMock
            .Setup(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()))
            .ReturnsAsync(false);

        _chemicalRepositoryMock
            .Setup(r => r.AddChemicalAsync(It.IsAny<Chemical>()))
            .ReturnsAsync((int?)null);

        // Act
        var result = await _service.AddChemicalAsync(chemicalDto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(ErrorMessage.DbError, result.Message);
        Assert.Equal(500, result.StatusCode);

        _chemicalRepositoryMock.Verify(r => r.AddChemicalAsync(It.IsAny<Chemical>()), Times.Once);
        _chemicalsNormalizationAdderServiceMock.Verify(s => s.AddHazardAsync(It.IsAny<IEnumerable<ChemicalsNormalization>>()), Times.Never);
    }

    [Fact]
    public async Task AddChemicalAsync_ShouldReturnHazardNotFound_WhenNoHazardsProvided()
    {
        // Arrange
        var chemicalDto = new ChemicalDtoAdd
        {
            ChemicalName = "Water",
            Concentration = "100%",
            Hazards = null
        };

        _chemicalRepositoryMock
            .Setup(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()))
            .ReturnsAsync(false);

        _chemicalRepositoryMock
            .Setup(r => r.AddChemicalAsync(It.IsAny<Chemical>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.AddChemicalAsync(chemicalDto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal("There are no such hazards on the database.", result.Message);
        Assert.Equal(404, result.StatusCode);

        _chemicalsNormalizationAdderServiceMock.Verify(s => s.AddHazardAsync(It.IsAny<IEnumerable<ChemicalsNormalization>>()), Times.Never);
    }
}
