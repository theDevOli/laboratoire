using Laboratoire.Application.Services.ChemicalServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ChemicalServices
{
    public class ChemicalUpdatableServiceTest
    {
        private readonly Mock<IChemicalRepository> _chemicalRepositoryMock;
        private readonly Mock<IChemicalsNormalizationAdderService> _normalizationAdderMock;
        private readonly Mock<ILogger<ChemicalUpdatableService>> _loggerMock;
        private readonly ChemicalUpdatableService _service;

        public ChemicalUpdatableServiceTest()
        {
            _chemicalRepositoryMock = new Mock<IChemicalRepository>();
            _normalizationAdderMock = new Mock<IChemicalsNormalizationAdderService>();
            _loggerMock = new Mock<ILogger<ChemicalUpdatableService>>();

            _service = new ChemicalUpdatableService(
                _chemicalRepositoryMock.Object,
                _normalizationAdderMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task UpdateChemicalAsync_ShouldReturnConflict_WhenChemicalNameAndConcentrationConflict()
        {
            var dto = new ChemicalDtoGetUpdate { ChemicalId = 1, ChemicalName = "Water", Concentration = "100%" };
            _chemicalRepositoryMock.Setup(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()))
                                   .ReturnsAsync(true);

            var result = await _service.UpdateChemicalAsync(dto);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(ErrorMessage.ConflictPost, result.Message);
            Assert.Equal(409, result.StatusCode);
            _chemicalRepositoryMock
            .Verify(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()), Times.Once);
            _chemicalRepositoryMock
            .Verify(r => r.DoesChemicalExistByIdAsync(It.IsAny<Chemical>()), Times.Never);
            _chemicalRepositoryMock
            .Verify(r => r.UpdateChemicalAsync(It.IsAny<Chemical>()), Times.Never);
            _normalizationAdderMock
            .Verify(r => r.AddHazardAsync(It.IsAny<ChemicalsNormalization[]>()), Times.Never);
        }

        [Fact]
        public async Task UpdateChemicalAsync_ShouldReturnNotFound_WhenChemicalDoesNotExist()
        {
            var dto = new ChemicalDtoGetUpdate { ChemicalId = 1, ChemicalName = "Water", Concentration = "100%" };
            _chemicalRepositoryMock.Setup(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()))
                                   .ReturnsAsync(false);
            _chemicalRepositoryMock.Setup(r => r.DoesChemicalExistByIdAsync(It.IsAny<Chemical>()))
                                   .ReturnsAsync(false);

            var result = await _service.UpdateChemicalAsync(dto);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(ErrorMessage.NotFound, result.Message);
            Assert.Equal(404, result.StatusCode);
            _chemicalRepositoryMock
            .Verify(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()), Times.Once);
            _chemicalRepositoryMock
            .Verify(r => r.DoesChemicalExistByIdAsync(It.IsAny<Chemical>()), Times.Once);
            _chemicalRepositoryMock
            .Verify(r => r.UpdateChemicalAsync(It.IsAny<Chemical>()), Times.Never);
            _normalizationAdderMock
            .Verify(r => r.AddHazardAsync(It.IsAny<ChemicalsNormalization[]>()), Times.Never);
        }

        [Fact]
        public async Task UpdateChemicalAsync_ShouldReturnDbError_WhenUpdateFails()
        {
            var dto = new ChemicalDtoGetUpdate { ChemicalId = 1, ChemicalName = "Water", Concentration = "100%" };
            _chemicalRepositoryMock.Setup(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()))
                                   .ReturnsAsync(false);
            _chemicalRepositoryMock.Setup(r => r.DoesChemicalExistByIdAsync(It.IsAny<Chemical>()))
                                   .ReturnsAsync(true);
            _chemicalRepositoryMock.Setup(r => r.UpdateChemicalAsync(It.IsAny<Chemical>()))
                                   .ReturnsAsync(false);

            var result = await _service.UpdateChemicalAsync(dto);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(ErrorMessage.DbError, result.Message);
            Assert.Equal(500, result.StatusCode);
            _chemicalRepositoryMock
            .Verify(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()), Times.Once);
            _chemicalRepositoryMock
            .Verify(r => r.DoesChemicalExistByIdAsync(It.IsAny<Chemical>()), Times.Once);
            _chemicalRepositoryMock
            .Verify(r => r.UpdateChemicalAsync(It.IsAny<Chemical>()), Times.Once);
            _normalizationAdderMock
            .Verify(r => r.AddHazardAsync(It.IsAny<ChemicalsNormalization[]>()), Times.Never);
        }

        [Fact]
        public async Task UpdateChemicalAsync_ShouldReturnSuccess_WhenNoHazardsProvided()
        {
            var dto = new ChemicalDtoGetUpdate { ChemicalId = 1, ChemicalName = "Water", Concentration = "100%", Hazards = null };
            _chemicalRepositoryMock.Setup(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()))
                                   .ReturnsAsync(false);
            _chemicalRepositoryMock.Setup(r => r.DoesChemicalExistByIdAsync(It.IsAny<Chemical>()))
                                   .ReturnsAsync(true);
            _chemicalRepositoryMock.Setup(r => r.UpdateChemicalAsync(It.IsAny<Chemical>()))
                                   .ReturnsAsync(true);

            var result = await _service.UpdateChemicalAsync(dto);

            Assert.False(result.IsNotSuccess());
            Assert.Null(result.Message);
            Assert.Equal(0, result.StatusCode);
            _chemicalRepositoryMock
            .Verify(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()), Times.Once);
            _chemicalRepositoryMock
            .Verify(r => r.DoesChemicalExistByIdAsync(It.IsAny<Chemical>()), Times.Once);
            _chemicalRepositoryMock
            .Verify(r => r.UpdateChemicalAsync(It.IsAny<Chemical>()), Times.Once);
            _normalizationAdderMock
            .Verify(r => r.AddHazardAsync(It.IsAny<ChemicalsNormalization[]>()), Times.Never);
        }

[Fact]
public async Task UpdateChemicalAsync_ShouldReturnSuccess_WhenHazardsUpdatedSuccessfully()
{
    var dto = new ChemicalDtoGetUpdate { ChemicalId = 1, ChemicalName = "Water", Concentration = "100%", Hazards = [1, 2] };
    
    _chemicalRepositoryMock.Setup(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()))
                           .ReturnsAsync(false);
    _chemicalRepositoryMock.Setup(r => r.DoesChemicalExistByIdAsync(It.IsAny<Chemical>()))
                           .ReturnsAsync(true);
    _chemicalRepositoryMock.Setup(r => r.UpdateChemicalAsync(It.IsAny<Chemical>()))
                           .ReturnsAsync(true);
    _normalizationAdderMock.Setup(s => s.AddHazardAsync(It.IsAny<IEnumerable<ChemicalsNormalization>>()))
                           .ReturnsAsync(Error.SetSuccess());

    var result = await _service.UpdateChemicalAsync(dto);

    Assert.False(result.IsNotSuccess());
    Assert.Null(result.Message);
    Assert.Equal(0, result.StatusCode);

    _normalizationAdderMock.Verify(r => r.AddHazardAsync(It.IsAny<IEnumerable<ChemicalsNormalization>>()), Times.Once);
}

[Fact]
public async Task UpdateChemicalAsync_ShouldReturnError_WhenHazardsUpdateFails()
{
    var dto = new ChemicalDtoGetUpdate { ChemicalId = 1, ChemicalName = "Water", Concentration = "100%", Hazards = [1] };
    
    _chemicalRepositoryMock.Setup(r => r.DoesChemicalExistByNameAndConcentrationAsync(It.IsAny<Chemical>()))
                           .ReturnsAsync(false);
    _chemicalRepositoryMock.Setup(r => r.DoesChemicalExistByIdAsync(It.IsAny<Chemical>()))
                           .ReturnsAsync(true);
    _chemicalRepositoryMock.Setup(r => r.UpdateChemicalAsync(It.IsAny<Chemical>()))
                           .ReturnsAsync(true);
    _normalizationAdderMock.Setup(s => s.AddHazardAsync(It.IsAny<IEnumerable<ChemicalsNormalization>>()))
                           .ReturnsAsync(Error.SetError("Some error", 400));

    var result = await _service.UpdateChemicalAsync(dto);

    Assert.True(result.IsNotSuccess());
    Assert.Equal("Some error", result.Message);
    Assert.Equal(400, result.StatusCode);

    _normalizationAdderMock.Verify(r => r.AddHazardAsync(It.IsAny<IEnumerable<ChemicalsNormalization>>()), Times.Once);
}

    }
}
