using Laboratoire.Application.Services.ChemicalServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ChemicalServices
{
    public class ChemicalsNormalizationGetterByIdServiceTest
    {
        private readonly Mock<IChemicalsNormalizationRepository> _repositoryMock;
        private readonly Mock<ILogger<ChemicalsNormalizationGetterByIdService>> _loggerMock;
        private readonly ChemicalsNormalizationGetterByIdService _service;

        public ChemicalsNormalizationGetterByIdServiceTest()
        {
            _repositoryMock = new Mock<IChemicalsNormalizationRepository>();
            _loggerMock = new Mock<ILogger<ChemicalsNormalizationGetterByIdService>>();
            _service = new ChemicalsNormalizationGetterByIdService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetHazardsByIdAsync_ShouldReturnNull_WhenChemicalIdIsNull()
        {
            // Act
            var result = await _service.GetHazardsByIdAsync(null);

            // Assert
            Assert.Null(result);
            _repositoryMock.Verify(r => r.GetHazardsByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetHazardsByIdAsync_ShouldReturnEmptyList_WhenNoHazardsExist()
        {
            // Arrange
            var chemicalId = 1;
            _repositoryMock.Setup(r => r.GetHazardsByIdAsync(chemicalId))
                           .ReturnsAsync(Array.Empty<ChemicalsNormalization>());

            // Act
            var result = await _service.GetHazardsByIdAsync(chemicalId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _repositoryMock.Verify(r => r.GetHazardsByIdAsync(chemicalId), Times.Once);
        }

        [Fact]
        public async Task GetHazardsByIdAsync_ShouldReturnHazards_WhenHazardsExist()
        {
            // Arrange
            var chemicalId = 1;
            var hazards = new[]
            {
                new ChemicalsNormalization { ChemicalId = chemicalId, HazardId = 1 },
                new ChemicalsNormalization { ChemicalId = chemicalId, HazardId = 2 }
            };
            _repositoryMock.Setup(r => r.GetHazardsByIdAsync(chemicalId))
                           .ReturnsAsync(hazards);

            // Act
            var result = await _service.GetHazardsByIdAsync(chemicalId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _repositoryMock.Verify(r => r.GetHazardsByIdAsync(chemicalId), Times.Once);
        }
    }
}
