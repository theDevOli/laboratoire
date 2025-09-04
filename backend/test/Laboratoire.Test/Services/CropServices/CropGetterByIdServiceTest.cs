using Laboratoire.Application.Services.CropServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CropServices
{
    public class CropGetterByIdServiceTest
    {
        private readonly Mock<ICropRepository> _cropRepositoryMock;
        private readonly Mock<ILogger<CropGetterByIdService>> _loggerMock;
        private readonly CropGetterByIdService _service;

        public CropGetterByIdServiceTest()
        {
            _cropRepositoryMock = new Mock<ICropRepository>();
            _loggerMock = new Mock<ILogger<CropGetterByIdService>>();
            _service = new CropGetterByIdService(_cropRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetCropByIdAsync_ShouldReturnNull_WhenCropIdIsNull()
        {
            var result = await _service.GetCropByIdAsync(null);

            Assert.Null(result);
            _cropRepositoryMock.Verify(r => r.GetCropByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetCropByIdAsync_ShouldReturnCrop_WhenCropIdIsValid()
        {
            var cropId = 1;
            var crop = new Crop { CropId = cropId, CropName = "Corn" };
            _cropRepositoryMock.Setup(r => r.GetCropByIdAsync(cropId)).ReturnsAsync(crop);

            var result = await _service.GetCropByIdAsync(cropId);

            Assert.NotNull(result);
            Assert.Equal(cropId, result!.CropId);
            Assert.Equal("Corn", result.CropName);

            _cropRepositoryMock.Verify(r => r.GetCropByIdAsync(cropId), Times.Once);
        }
    }
}
