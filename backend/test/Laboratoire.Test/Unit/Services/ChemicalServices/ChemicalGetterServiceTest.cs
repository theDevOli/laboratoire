using Laboratoire.Application.Services.ChemicalServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ChemicalServices
{
    public class ChemicalGetterServiceTest
    {
        private readonly Mock<IChemicalRepository> _chemicalRepositoryMock;
        private readonly Mock<IChemicalsNormalizationRepository> _chemicalsNormalizationRepositoryMock;
        private readonly Mock<ILogger<ChemicalGetterService>> _loggerMock;
        private readonly ChemicalGetterService _service;

        public ChemicalGetterServiceTest()
        {
            _chemicalRepositoryMock = new Mock<IChemicalRepository>();
            _chemicalsNormalizationRepositoryMock = new Mock<IChemicalsNormalizationRepository>();
            _loggerMock = new Mock<ILogger<ChemicalGetterService>>();

            _service = new ChemicalGetterService(
                _chemicalRepositoryMock.Object,
                _chemicalsNormalizationRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAllChemicalsAsync_ShouldReturnAllChemicalsWithHazards()
        {
            // Arrange
            var chemicals = new List<Chemical>
            {
                new Chemical { ChemicalId = 1, ChemicalName = "Water", Concentration = "100%" },
                new Chemical { ChemicalId = 2, ChemicalName = "Ethanol", Concentration = "95%" }
            };

            var hazards = new List<ChemicalsNormalization>
            {
                new ChemicalsNormalization { ChemicalId = 1, HazardId = 1 },
                new ChemicalsNormalization { ChemicalId = 1, HazardId = 2 },
                new ChemicalsNormalization { ChemicalId = 2, HazardId = 3 }
            };

            _chemicalRepositoryMock
                .Setup(r => r.GetAllChemicalsAsync())
                .ReturnsAsync(chemicals);

            _chemicalsNormalizationRepositoryMock
                .Setup(r => r.GetAllHazardsAsync())
                .ReturnsAsync(hazards);

            // Act
            var result = await _service.GetAllChemicalsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var water = result.First(c => c.ChemicalId == 1);
            Assert.Contains(1, water.Hazards);
            Assert.Contains(2, water.Hazards);

            var ethanol = result.First(c => c.ChemicalId == 2);
            Assert.Contains(3, ethanol.Hazards);

            _chemicalRepositoryMock.Verify(r => r.GetAllChemicalsAsync(), Times.Once);
            _chemicalsNormalizationRepositoryMock.Verify(r => r.GetAllHazardsAsync(), Times.Once);
        }
    }
}
