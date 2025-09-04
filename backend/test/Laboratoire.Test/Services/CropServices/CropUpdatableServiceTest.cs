using Laboratoire.Application.Services.CropServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CropServices
{
    public class CropUpdatableServiceTest
    {
        private readonly Mock<ICropRepository> _repositoryMock;
        private readonly Mock<ILogger<CropUpdatableService>> _loggerMock;
        private readonly CropUpdatableService _service;

        public CropUpdatableServiceTest()
        {
            _repositoryMock = new Mock<ICropRepository>();
            _loggerMock = new Mock<ILogger<CropUpdatableService>>();
            _service = new CropUpdatableService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task UpdateCropAsync_ShouldReturnConflict_WhenCropNameExists()
        {
            // Arrange
            var crop = new Crop { CropId = 1, CropName = "Wheat" };
            _repositoryMock.Setup(r => r.DoesCropExistByNameAsync(crop)).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateCropAsync(crop);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(409, result.StatusCode);
            Assert.Equal(ErrorMessage.ConflictPut, result.Message);
            _repositoryMock.Verify(r => r.DoesCropExistByNameAsync(It.IsAny<Crop>()), Times.Once);
            _repositoryMock.Verify(r => r.DoesCropExistByCropIdAsync(It.IsAny<Crop>()), Times.Never);
            _repositoryMock.Verify(r => r.UpdateCropAsync(It.IsAny<Crop>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCropAsync_ShouldReturnNotFound_WhenCropDoesNotExistById()
        {
            // Arrange
            var crop = new Crop { CropId = 2, CropName = "Corn" };
            _repositoryMock.Setup(r => r.DoesCropExistByNameAsync(crop)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.DoesCropExistByCropIdAsync(crop)).ReturnsAsync(false);

            // Act
            var result = await _service.UpdateCropAsync(crop);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
            Assert.Equal(ErrorMessage.NotFound, result.Message);
            _repositoryMock.Verify(r => r.DoesCropExistByNameAsync(It.IsAny<Crop>()), Times.Once);
            _repositoryMock.Verify(r => r.DoesCropExistByCropIdAsync(It.IsAny<Crop>()), Times.Once);
            _repositoryMock.Verify(r => r.UpdateCropAsync(It.IsAny<Crop>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCropAsync_ShouldReturnDbError_WhenUpdateFails()
        {
            // Arrange
            var crop = new Crop { CropId = 3, CropName = "Rice" };
            _repositoryMock.Setup(r => r.DoesCropExistByNameAsync(crop)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.DoesCropExistByCropIdAsync(crop)).ReturnsAsync(true);
            _repositoryMock.Setup(r => r.UpdateCropAsync(crop)).ReturnsAsync(false);

            // Act
            var result = await _service.UpdateCropAsync(crop);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _repositoryMock.Verify(r => r.DoesCropExistByNameAsync(It.IsAny<Crop>()), Times.Once);
            _repositoryMock.Verify(r => r.DoesCropExistByCropIdAsync(It.IsAny<Crop>()), Times.Once);
            _repositoryMock.Verify(r => r.UpdateCropAsync(It.IsAny<Crop>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCropAsync_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            // Arrange
            var crop = new Crop { CropId = 4, CropName = "Soybean" };
            _repositoryMock.Setup(r => r.DoesCropExistByNameAsync(crop)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.DoesCropExistByCropIdAsync(crop)).ReturnsAsync(true);
            _repositoryMock.Setup(r => r.UpdateCropAsync(crop)).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateCropAsync(crop);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null( result.Message);
            _repositoryMock.Verify(r => r.DoesCropExistByNameAsync(It.IsAny<Crop>()), Times.Once);
            _repositoryMock.Verify(r => r.DoesCropExistByCropIdAsync(It.IsAny<Crop>()), Times.Once);
            _repositoryMock.Verify(r => r.UpdateCropAsync(It.IsAny<Crop>()), Times.Once);
        }
    }
}
