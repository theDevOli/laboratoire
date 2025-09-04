using Laboratoire.Application.Services.HazardServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.HazardServices
{
    public class HazardGetterServiceTest
    {
        private readonly Mock<IHazardRepository> _repositoryMock;
        private readonly Mock<ILogger<HazardGetterService>> _loggerMock;
        private readonly HazardGetterService _service;

        public HazardGetterServiceTest()
        {
            _repositoryMock = new Mock<IHazardRepository>();
            _loggerMock = new Mock<ILogger<HazardGetterService>>();
            _service = new HazardGetterService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAllHazardsAsync_ShouldReturnEmptyList_WhenNoHazardsExist()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllHazardsAsync())
                           .ReturnsAsync(Enumerable.Empty<Hazard>());

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
            var hazards = new List<Hazard>
            {
                new Hazard { HazardId = 1, HazardClass = "Flammable" },
                new Hazard { HazardId = 2, HazardClass = "Toxic" }
            };

            _repositoryMock.Setup(r => r.GetAllHazardsAsync())
                           .ReturnsAsync(hazards);

            // Act
            var result = await _service.GetAllHazardsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Collection
            (
                result,
                item => Assert.Equal(item.HazardId, hazards[0].HazardId),
                item => Assert.Equal(item.HazardId, hazards[1].HazardId)
            );
            _repositoryMock.Verify(r => r.GetAllHazardsAsync(), Times.Once);
        }
    }
}
