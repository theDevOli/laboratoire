using Laboratoire.Application.Services.UtilServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.UtilServices
{
    public class FertilizerGetterServiceTest
    {
        private readonly Mock<IFertilizerRepository> _fertilizerRepoMock;
        private readonly Mock<ILogger<FertilizerGetterService>> _loggerMock;
        private readonly FertilizerGetterService _service;

        public FertilizerGetterServiceTest()
        {
            _fertilizerRepoMock = new Mock<IFertilizerRepository>();
            _loggerMock = new Mock<ILogger<FertilizerGetterService>>();
            _service = new FertilizerGetterService(_fertilizerRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllFertilizersAsync_ShouldReturnDtoList_WhenFertilizeExists()
        {
            // Arrange
            var fertilizers = new List<Fertilizer>
            {
                new() { FertilizerId = 1 },
                new() { FertilizerId = 2 }
            };

            _fertilizerRepoMock.Setup(r => r.GetAllFertilizersAsync())
                               .ReturnsAsync(fertilizers);

            // Act
            var result = await _service.GetAllFertilizersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Collection
            (
                result,
                item => Assert.Equal(item.FertilizerId, fertilizers[0].FertilizerId),
                item => Assert.Equal(item.FertilizerId, fertilizers[1].FertilizerId)
            );
            _fertilizerRepoMock.Verify(r => r.GetAllFertilizersAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllFertilizersAsync_ShouldReturnEmpty_WhenNoFertilizeExists()
        {
            // Arrange
            _fertilizerRepoMock.Setup(r => r.GetAllFertilizersAsync())
                               .ReturnsAsync(Enumerable.Empty<Fertilizer>());

            // Act
            var result = await _service.GetAllFertilizersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _fertilizerRepoMock.Verify(r => r.GetAllFertilizersAsync(), Times.Once);
        }
    }
}
