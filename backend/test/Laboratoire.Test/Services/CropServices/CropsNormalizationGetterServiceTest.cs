using Laboratoire.Application.Services.CropServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CropServices
{
    public class CropsNormalizationGetterServiceTest
    {
        private readonly Mock<ICropsNormalizationRepository> _repositoryMock;
        private readonly Mock<ILogger<CropsNormalizationGetterService>> _loggerMock;
        private readonly CropsNormalizationGetterService _service;

        public CropsNormalizationGetterServiceTest()
        {
            _repositoryMock = new Mock<ICropsNormalizationRepository>();
            _loggerMock = new Mock<ILogger<CropsNormalizationGetterService>>();
            _service = new CropsNormalizationGetterService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAllCropsAsync_ShouldReturnAllCrops_WhenCropsExist()
        {
            // Arrange
            var crops = new List<CropsNormalization>
            {
                new CropsNormalization { ProtocolId = "P1" },
                new CropsNormalization { ProtocolId = "P2" }
            };

            _repositoryMock
                .Setup(r => r.GetAllCropsAsync())
                .ReturnsAsync(crops);

            // Act
            var result = await _service.GetAllCropsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Collection
            (
                result,
                item => Assert.Equal(item.ProtocolId, crops[0].ProtocolId),
                item => Assert.Equal(item.ProtocolId, crops[1].ProtocolId)
            );

            _repositoryMock.Verify(r => r.GetAllCropsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllCropsAsync_ShouldReturnEmptyCollect_WhenCropsDontExist()
        {
            // Arrange
            var crops = Enumerable.Empty<CropsNormalization>();

            _repositoryMock
                .Setup(r => r.GetAllCropsAsync())
                .ReturnsAsync(crops);

            // Act
            var result = await _service.GetAllCropsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _repositoryMock.Verify(r => r.GetAllCropsAsync(), Times.Once);
        }
    }
}
