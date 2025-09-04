using Laboratoire.Application.Services.CropServices;
using Laboratoire.Application.DTO;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Test.Services.CropServices
{
    public class CropAdderServiceTest
    {
        private readonly Mock<ICropRepository> _cropRepositoryMock;
        private readonly Mock<ILogger<CropAdderService>> _loggerMock;
        private readonly CropAdderService _service;

        public CropAdderServiceTest()
        {
            _cropRepositoryMock = new Mock<ICropRepository>();
            _loggerMock = new Mock<ILogger<CropAdderService>>();
            _service = new CropAdderService(_cropRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddCropAsync_ShouldReturnConflict_WhenCropAlreadyExists()
        {
            var cropDto = new CropDtoAdd { CropName = "Corn" };
            _cropRepositoryMock.Setup(r => r.DoesCropExistByNameAsync(It.IsAny<Crop>()))
                               .ReturnsAsync(true);

            var result = await _service.AddCropAsync(cropDto);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(409, result.StatusCode);
            Assert.Equal(ErrorMessage.ConflictPost, result.Message);
            _cropRepositoryMock.Verify(r => r.DoesCropExistByNameAsync(It.IsAny<Crop>()), Times.Once);
            _cropRepositoryMock.Verify(r => r.AddCropAsync(It.IsAny<Crop>()), Times.Never);
        }

        [Fact]
        public async Task AddCropAsync_ShouldReturnDbError_WhenAddFails()
        {
            var cropDto = new CropDtoAdd { CropName = "Wheat" };
            _cropRepositoryMock.Setup(r => r.DoesCropExistByNameAsync(It.IsAny<Crop>()))
                               .ReturnsAsync(false);
            _cropRepositoryMock.Setup(r => r.AddCropAsync(It.IsAny<Crop>()))
                               .ReturnsAsync(false);

            var result = await _service.AddCropAsync(cropDto);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _cropRepositoryMock.Verify(r => r.DoesCropExistByNameAsync(It.IsAny<Crop>()), Times.Once);
            _cropRepositoryMock.Verify(r => r.AddCropAsync(It.IsAny<Crop>()), Times.Once);
        }

        [Fact]
        public async Task AddCropAsync_ShouldReturnSuccess_WhenAddSucceeds()
        {
            var cropDto = new CropDtoAdd { CropName = "Rice" };
            _cropRepositoryMock.Setup(r => r.DoesCropExistByNameAsync(It.IsAny<Crop>()))
                               .ReturnsAsync(false);
            _cropRepositoryMock.Setup(r => r.AddCropAsync(It.IsAny<Crop>()))
                               .ReturnsAsync(true);

            var result = await _service.AddCropAsync(cropDto);

            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null( result.Message);
            _cropRepositoryMock.Verify(r => r.DoesCropExistByNameAsync(It.IsAny<Crop>()), Times.Once);
            _cropRepositoryMock.Verify(r => r.AddCropAsync(It.IsAny<Crop>()), Times.Once);
        }
    }
}
