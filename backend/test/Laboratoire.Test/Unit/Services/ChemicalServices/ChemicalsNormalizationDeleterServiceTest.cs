using Laboratoire.Application.Services.ChemicalServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ChemicalServices
{
    public class ChemicalsNormalizationDeleterServiceTest
    {
        private readonly Mock<IChemicalsNormalizationRepository> _repositoryMock;
        private readonly Mock<ILogger<ChemicalsNormalizationDeleterService>> _loggerMock;
        private readonly ChemicalsNormalizationDeleterService _service;

        public ChemicalsNormalizationDeleterServiceTest()
        {
            _repositoryMock = new Mock<IChemicalsNormalizationRepository>();
            _loggerMock = new Mock<ILogger<ChemicalsNormalizationDeleterService>>();

            _service = new ChemicalsNormalizationDeleterService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task DeleteHazardAsync_ShouldReturnBadRequest_WhenChemicalIdIsNull()
        {
            // Act
            var result = await _service.DeleteHazardAsync(null);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(ErrorMessage.BadRequest, result.Message);
            Assert.Equal(400, result.StatusCode);
            _repositoryMock.Verify(r => r.CountHazardAsync(It.IsAny<int>()), Times.Never);
            _repositoryMock.Verify(r => r.DeleteHazardAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteHazardAsync_ShouldReturnSuccess_WhenNoHazardsExist()
        {
            // Arrange
            var chemicalId = 1;
            _repositoryMock.Setup(r => r.CountHazardAsync(chemicalId)).ReturnsAsync(0);

            // Act
            var result = await _service.DeleteHazardAsync(chemicalId);

            // Assert
            Assert.False(result.IsNotSuccess());
            _repositoryMock.Verify(r => r.CountHazardAsync(chemicalId), Times.Once);
            _repositoryMock.Verify(r => r.DeleteHazardAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteHazardAsync_ShouldReturnSuccess_WhenDeletionSucceeds()
        {
            // Arrange
            var chemicalId = 1;
            _repositoryMock.Setup(r => r.CountHazardAsync(chemicalId)).ReturnsAsync(2);
            _repositoryMock.Setup(r => r.DeleteHazardAsync(chemicalId)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteHazardAsync(chemicalId);

            // Assert
            Assert.False(result.IsNotSuccess());
            _repositoryMock.Verify(r => r.CountHazardAsync(chemicalId), Times.Once);
            _repositoryMock.Verify(r => r.DeleteHazardAsync(chemicalId), Times.Once);
        }

        [Fact]
        public async Task DeleteHazardAsync_ShouldReturnDbError_WhenDeletionFails()
        {
            // Arrange
            var chemicalId = 1;
            _repositoryMock.Setup(r => r.CountHazardAsync(chemicalId)).ReturnsAsync(2);
            _repositoryMock.Setup(r => r.DeleteHazardAsync(chemicalId)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteHazardAsync(chemicalId);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(ErrorMessage.DbError, result.Message);
            Assert.Equal(500, result.StatusCode);
            _repositoryMock.Verify(r => r.CountHazardAsync(chemicalId), Times.Once);
            _repositoryMock.Verify(r => r.DeleteHazardAsync(chemicalId), Times.Once);
        }
    }
}
