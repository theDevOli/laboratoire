using Laboratoire.Application.Services.CropServices;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CropServices
{
    public class CropsNormalizationDeleterServiceTest
    {
        private readonly Mock<ICropsNormalizationRepository> _repositoryMock;
        private readonly Mock<ILogger<CropsNormalizationDeleterService>> _loggerMock;
        private readonly CropsNormalizationDeleterService _service;

        public CropsNormalizationDeleterServiceTest()
        {
            _repositoryMock = new Mock<ICropsNormalizationRepository>();
            _loggerMock = new Mock<ILogger<CropsNormalizationDeleterService>>();
            _service = new CropsNormalizationDeleterService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task DeleteCropsAsync_ShouldReturnError_WhenProtocolIdIsNullOrEmpty(string? protocolId)
        {
            // Act
            var result = await _service.DeleteCropsAsync(protocolId);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(400, result.StatusCode);
            _repositoryMock.Verify(r => r.IsThereNoneCropsAsync(It.IsAny<string>()), Times.Never);
            _repositoryMock.Verify(r => r.DeleteCropsAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCropsAsync_ShouldReturnSuccess_WhenNoCropsExist()
        {
            // Arrange
            string protocolId = "P1";
            _repositoryMock.Setup(r => r.IsThereNoneCropsAsync(protocolId)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteCropsAsync(protocolId);

            // Assert
            Assert.False(result.IsNotSuccess());
            _repositoryMock.Verify(r => r.IsThereNoneCropsAsync(protocolId), Times.Once);
            _repositoryMock.Verify(r => r.DeleteCropsAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCropsAsync_ShouldReturnError_WhenDeletionFails()
        {
            // Arrange
            string protocolId = "P1";
            _repositoryMock.Setup(r => r.IsThereNoneCropsAsync(protocolId)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.DeleteCropsAsync(protocolId)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteCropsAsync(protocolId);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            _repositoryMock.Verify(r => r.IsThereNoneCropsAsync(It.IsAny<string>()), Times.Once);
            _repositoryMock.Verify(r => r.DeleteCropsAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCropsAsync_ShouldReturnSuccess_WhenDeletionSucceeds()
        {
            // Arrange
            string protocolId = "P1";
            _repositoryMock.Setup(r => r.IsThereNoneCropsAsync(protocolId)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.DeleteCropsAsync(protocolId)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteCropsAsync(protocolId);

            // Assert
            Assert.False(result.IsNotSuccess());
            _repositoryMock.Verify(r => r.IsThereNoneCropsAsync(It.IsAny<string>()), Times.Once);
            _repositoryMock.Verify(r => r.DeleteCropsAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
