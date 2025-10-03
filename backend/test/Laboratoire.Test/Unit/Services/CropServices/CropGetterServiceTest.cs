using System.Collections;
using Laboratoire.Application.Services.CropServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CropServices
{
    public class CropGetterServiceTest
    {
        private readonly Mock<ICropRepository> _cropRepositoryMock;
        private readonly Mock<ILogger<CropGetterService>> _loggerMock;
        private readonly CropGetterService _service;

        public CropGetterServiceTest()
        {
            _cropRepositoryMock = new Mock<ICropRepository>();
            _loggerMock = new Mock<ILogger<CropGetterService>>();
            _service = new CropGetterService(_cropRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllCropsAsync_ShouldReturnAllCrops_WhenCropsExist()
        {
            // Arrange
            var crops = new List<Crop>
            {
                new Crop { CropId = 1, CropName = "Corn" },
                new Crop { CropId = 2, CropName = "Wheat" }
            };
            _cropRepositoryMock.Setup(r => r.GetAllCropsAsync()).ReturnsAsync(crops);

            // Act
            var result = await _service.GetAllCropsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Collection(result,
                item => Assert.Equal("Corn", item.CropName),
                item => Assert.Equal("Wheat", item.CropName)
            );

            _cropRepositoryMock.Verify(r => r.GetAllCropsAsync(), Times.Once);
        }
        [Fact]
        public async Task GetAllCropsAsync_ShouldReturnEmptyCollection_WhenCropsDontExist()
        {

            // Arrange
            _cropRepositoryMock.Setup(r => r.GetAllCropsAsync()).ReturnsAsync(Enumerable.Empty<Crop>());

            // Act
            var result = await _service.GetAllCropsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _cropRepositoryMock.Verify(r => r.GetAllCropsAsync(), Times.Once);
        }
    }
}
