using Laboratoire.Application.Services.ChemicalServices;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ChemicalServices
{
    public class ChemicalsNormalizationAdderServiceTest
    {
        private readonly Mock<IChemicalsNormalizationRepository> _repositoryMock;
        private readonly Mock<IChemicalsNormalizationDeleterService> _deleterServiceMock;
        private readonly Mock<ILogger<ChemicalsNormalizationAdderService>> _loggerMock;
        private readonly ChemicalsNormalizationAdderService _service;

        public ChemicalsNormalizationAdderServiceTest()
        {
            _repositoryMock = new Mock<IChemicalsNormalizationRepository>();
            _deleterServiceMock = new Mock<IChemicalsNormalizationDeleterService>();
            _loggerMock = new Mock<ILogger<ChemicalsNormalizationAdderService>>();

            _service = new ChemicalsNormalizationAdderService(
                _repositoryMock.Object,
                _deleterServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task AddHazardAsync_ShouldReturnSuccess_WhenAllOperationsSucceed()
        {
            // Arrange
            var hazards = new[]
            {
                new ChemicalsNormalization { ChemicalId = 1, HazardId = 1 },
                new ChemicalsNormalization { ChemicalId = 1, HazardId = 2 }
            };

            _deleterServiceMock
                .Setup(d => d.DeleteHazardAsync(1))
                .ReturnsAsync(Error.SetSuccess());

            _repositoryMock
                .Setup(r => r.AddHazardAsync(hazards))
                .ReturnsAsync(true);

            // Act
            var result = await _service.AddHazardAsync(hazards);

            // Assert
            Assert.False(result.IsNotSuccess());
            _deleterServiceMock.Verify(d => d.DeleteHazardAsync(1), Times.Once);
            _repositoryMock.Verify(r => r.AddHazardAsync(hazards), Times.Once);
        }

        [Fact]
        public async Task AddHazardAsync_ShouldReturnBadRequest_WhenChemicalIdIsNull()
        {
            // Arrange
            var hazards = new[]
            {
                new ChemicalsNormalization { ChemicalId = null, HazardId = 1 }
            };

            // Act
            var result = await _service.AddHazardAsync(hazards);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(ErrorMessage.BadRequestFirstIdNull, result.Message);
            Assert.Equal(400, result.StatusCode);
            _deleterServiceMock.Verify(d => d.DeleteHazardAsync(It.IsAny<int>()), Times.Never);
            _repositoryMock.Verify(r => r.AddHazardAsync(It.IsAny<IEnumerable<ChemicalsNormalization>>()), Times.Never);
        }

        [Fact]
        public async Task AddHazardAsync_ShouldReturnBadRequest_WhenHazardIdIsNull()
        {
            // Arrange
            var hazards = new[]
            {
                new ChemicalsNormalization { ChemicalId = 1, HazardId = null }
            };

            // Act
            var result = await _service.AddHazardAsync(hazards);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(ErrorMessage.BadRequestIdNotNull, result.Message);
            Assert.Equal(400, result.StatusCode);
            _deleterServiceMock.Verify(d => d.DeleteHazardAsync(It.IsAny<int>()), Times.Never);
            _repositoryMock.Verify(r => r.AddHazardAsync(It.IsAny<IEnumerable<ChemicalsNormalization>>()), Times.Never);
        }

        [Fact]
        public async Task AddHazardAsync_ShouldReturnError_WhenDeletionFails()
        {
            // Arrange
            var hazards = new[]
            {
                new ChemicalsNormalization { ChemicalId = 1, HazardId = 1 }
            };

            _deleterServiceMock
                .Setup(d => d.DeleteHazardAsync(1))
                .ReturnsAsync(Error.SetError("Deletion failed", 500));

            // Act
            var result = await _service.AddHazardAsync(hazards);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal("Deletion failed", result.Message);
            _repositoryMock.Verify(r => r.AddHazardAsync(It.IsAny<IEnumerable<ChemicalsNormalization>>()), Times.Never);
        }

        [Fact]
        public async Task AddHazardAsync_ShouldReturnError_WhenAddHazardFails()
        {
            // Arrange
            var hazards = new[]
            {
                new ChemicalsNormalization { ChemicalId = 1, HazardId = 1 }
            };

            _deleterServiceMock
                .Setup(d => d.DeleteHazardAsync(1))
                .ReturnsAsync(Error.SetSuccess());

            _repositoryMock
                .Setup(r => r.AddHazardAsync(hazards))
                .ReturnsAsync(false);

            // Act
            var result = await _service.AddHazardAsync(hazards);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(ErrorMessage.IDOutRange, result.Message);
        }
    }
}
