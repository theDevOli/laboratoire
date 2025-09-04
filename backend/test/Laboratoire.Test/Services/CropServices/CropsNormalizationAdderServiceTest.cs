using Laboratoire.Application.Services.CropServices;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CropServices
{
    public class CropsNormalizationAdderServiceTest
    {
        private readonly Mock<ICropsNormalizationRepository> _repositoryMock;
        private readonly Mock<ICropsNormalizationDeleterService> _deleterMock;
        private readonly Mock<ILogger<CropsNormalizationAdderService>> _loggerMock;
        private readonly CropsNormalizationAdderService _service;

        public CropsNormalizationAdderServiceTest()
        {
            _repositoryMock = new Mock<ICropsNormalizationRepository>();
            _deleterMock = new Mock<ICropsNormalizationDeleterService>();
            _loggerMock = new Mock<ILogger<CropsNormalizationAdderService>>();
            _service = new CropsNormalizationAdderService(_repositoryMock.Object, _deleterMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddCropsAsync_ShouldReturnError_WhenDeletionFails()
        {
            // Arrange
            string protocolId = "P1";
            _deleterMock.Setup(d => d.DeleteCropsAsync(protocolId))
                        .ReturnsAsync(Error.SetError("Delete failed", 400));

            // Act
            var result = await _service.AddCropsAsync(null, protocolId);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(400, result.StatusCode);
            _deleterMock.Verify(d => d.DeleteCropsAsync(It.IsAny<string>()), Times.Once);
            _repositoryMock.Verify(r => r.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>()), Times.Never);
        }

        [Fact]
        public async Task AddCropsAsync_ShouldReturnSuccess_WhenNoCropsProvided()
        {
            // Arrange
            string protocolId = "P1";
            _deleterMock.Setup(d => d.DeleteCropsAsync(protocolId))
                        .ReturnsAsync(Error.SetSuccess());

            // Act
            var result = await _service.AddCropsAsync(null, protocolId);

            // Assert
            Assert.False(result.IsNotSuccess());
            _deleterMock.Verify(d => d.DeleteCropsAsync(It.IsAny<string>()), Times.Once);
            _repositoryMock.Verify(r => r.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>()), Times.Never);
        }

        [Fact]
        public async Task AddCropsAsync_ShouldReturnError_WhenAdditionFails()
        {
            // Arrange
            string protocolId = "P1";
            var crops = new List<CropsNormalization> { new() { ProtocolId = protocolId, CropId = 1 } };

            _deleterMock.Setup(d => d.DeleteCropsAsync(protocolId)).ReturnsAsync(Error.SetSuccess());
            _repositoryMock.Setup(r => r.AddCropsAsync(crops)).ReturnsAsync(false);

            // Act
            var result = await _service.AddCropsAsync(crops, protocolId);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            _deleterMock.Verify(d => d.DeleteCropsAsync(It.IsAny<string>()), Times.Once);
            _repositoryMock.Verify(r => r.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>()), Times.Once);
        }

        [Fact]
        public async Task AddCropsAsync_ShouldReturnSuccess_WhenAdditionSucceeds()
        {
            // Arrange
            string protocolId = "P1";
            var crops = new List<CropsNormalization> { new() { ProtocolId = protocolId, CropId = 1 } };

            _deleterMock.Setup(d => d.DeleteCropsAsync(protocolId)).ReturnsAsync(Error.SetSuccess());
            _repositoryMock.Setup(r => r.AddCropsAsync(crops)).ReturnsAsync(true);

            // Act
            var result = await _service.AddCropsAsync(crops, protocolId);

            // Assert
            Assert.False(result.IsNotSuccess());
            _deleterMock.Verify(d => d.DeleteCropsAsync(It.IsAny<string>()), Times.Once);
            _repositoryMock.Verify(r => r.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>()), Times.Once);
        }
    }
}
