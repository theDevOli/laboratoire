using Laboratoire.Application.Services.ChemicalServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ChemicalServices
{
    public class ChemicalsNormalizationGetterServiceTest
    {
        private readonly Mock<IChemicalsNormalizationRepository> _repositoryMock;
        private readonly Mock<ILogger<ChemicalsNormalizationGetterService>> _loggerMock;
        private readonly ChemicalsNormalizationGetterService _service;

        public ChemicalsNormalizationGetterServiceTest()
        {
            _repositoryMock = new Mock<IChemicalsNormalizationRepository>();
            _loggerMock = new Mock<ILogger<ChemicalsNormalizationGetterService>>();
            _service = new ChemicalsNormalizationGetterService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAllHazardsAsync_ShouldReturnEmptyList_WhenNoHazardsExist()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllHazardsAsync())
                           .ReturnsAsync(Array.Empty<ChemicalsNormalization>());

            // Act
            var result = await _service.GetAllHazardsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _repositoryMock.Verify(r => r.GetAllHazardsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllHazardsAsync_ShouldReturnHazards_WhenHazardsExist()
        {
            // Arrange
            var hazards = new[]
            {
                new ChemicalsNormalization { ChemicalId = 1, HazardId = 1 },
                new ChemicalsNormalization { ChemicalId = 2, HazardId = 2 }
            };
            _repositoryMock.Setup(r => r.GetAllHazardsAsync())
                           .ReturnsAsync(hazards);

            // Act
            var result = await _service.GetAllHazardsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _repositoryMock.Verify(r => r.GetAllHazardsAsync(), Times.Once);
        }
    }
}
