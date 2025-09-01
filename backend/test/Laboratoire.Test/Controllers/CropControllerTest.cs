using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.UI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Laboratoire.Tests
{
    public class CropControllerTests
    {
        private readonly Mock<ICropGetterService> _mockCropGetterService;
        private readonly Mock<ICropGetterByIdService> _mockCropGetterByIdService;
        private readonly Mock<ICropAdderService> _mockCropAdderService;
        private readonly Mock<ICropUpdatableService> _mockCropUpdatableService;
        private readonly CropController _controller;

        public CropControllerTests()
        {
            _mockCropGetterService = new Mock<ICropGetterService>();
            _mockCropGetterByIdService = new Mock<ICropGetterByIdService>();
            _mockCropAdderService = new Mock<ICropAdderService>();
            _mockCropUpdatableService = new Mock<ICropUpdatableService>();
            _controller = new CropController(
                _mockCropGetterService.Object,
                _mockCropGetterByIdService.Object,
                _mockCropAdderService.Object,
                _mockCropUpdatableService.Object
            );
        }

        [Fact]
        public async Task GetAllCropsAsync_ShouldReturnOk_WhenCropsExist()
        {
            // Arrange
            var crops = new List<Crop> { new Crop { CropId = 1, CropName = "Corn" } };
            _mockCropGetterService.Setup(service => service.GetAllCropsAsync()).ReturnsAsync(crops);

            // Act
            var result = await _controller.GetAllCropsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<Crop>>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
            Assert.Null(apiResponse.Error);
        }

        [Fact]
        public async Task GetAllCropsAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            _mockCropGetterService.Setup(repo => repo.GetAllCropsAsync()).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAllCropsAsync();

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task GetCropByIdAsync_ShouldReturnOk_WhenCropExists()
        {
            // Arrange
            var crop = new Crop { CropId = 1, CropName = "Corn" };
            _mockCropGetterByIdService.Setup(service => service.GetCropByIdAsync(It.IsAny<int>())).ReturnsAsync(crop);

            // Act
            var result = await _controller.GetCropByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Crop>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(crop.CropName, apiResponse.Data.CropName);
        }

        [Fact]
        public async Task GetCropByIdAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            _mockCropGetterByIdService.Setup(repo => repo.GetCropByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetCropByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task GetCropByIdAsync_ShouldReturnNotFound_WhenCropDoesNotExist()
        {
            // Arrange
            Crop? crop = null;
            // var crop = new CropDtoAdd { CropName = "Corn" };
            _mockCropGetterByIdService.Setup(service => service.GetCropByIdAsync(It.IsAny<int>())).ReturnsAsync(crop);

            // Act
            var result = await _controller.GetCropByIdAsync(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal(404, apiResponse.Error?.Code);
            Assert.Equal(ErrorMessage.NotFound, apiResponse.Error?.Message);
            Assert.Null(apiResponse.Data);
        }

        [Fact]
        public async Task AddCropAsync_ShouldReturnCreated_WhenCropIsAdded()
        {
            // Arrange
            var expectedError = Error.SetSuccess();
            var cropDto = new CropDtoAdd { CropName = "Corn", NitrogenCover = 50 };
            _mockCropAdderService.Setup(service => service.AddCropAsync(It.IsAny<CropDtoAdd>()))
                                 .ReturnsAsync(expectedError);

            // Act
            var result = await _controller.AddCropAsync(cropDto);

            // Assert
            var createdResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse<string>>(createdResult.Value);
            Assert.Equal(SuccessMessage.Added, apiResponse.Data);
        }

        [Fact]
        public async Task AddCropAsync_ShouldReturnConflict_WhenInvalidCropIsAdded()
        {
            // Arrange
            var expectedError = Error.SetError(ErrorMessage.ConflictPost, 409);
            var cropDto = new CropDtoAdd { CropName = "Corn", NitrogenCover = 50 };
            _mockCropAdderService.Setup(service => service.AddCropAsync(It.IsAny<CropDtoAdd>()))
                                 .ReturnsAsync(expectedError);

            // Act
            var result = await _controller.AddCropAsync(cropDto);

            // Assert
            var conflictResult = Assert.IsType<ObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(conflictResult.Value);
            Assert.Equal(409, apiResponse.Error?.Code);
            Assert.Equal(ErrorMessage.ConflictPost, apiResponse.Error?.Message);
            Assert.Null(apiResponse.Data);
        }

        [Fact]
        public async Task AddCropAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            var cropDto = new CropDtoAdd { CropName = "Corn", NitrogenCover = 50 };
            _mockCropAdderService.Setup(repo => repo.AddCropAsync(It.IsAny<CropDtoAdd>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.AddCropAsync(cropDto);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task UpdateCropAsync_ShouldReturnBadRequest_WhenCropIdsDoNotMatch()
        {
            // Arrange
            var crop = new Crop { CropId = 1, CropName = "Corn" };
            var cropId = 2;

            // Act
            var result = await _controller.UpdateCropAsync(crop, cropId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal(400, apiResponse.Error?.Code);
            Assert.Equal(ErrorMessage.BadRequestID, apiResponse.Error?.Message);
            Assert.Null(apiResponse.Data);
        }

        [Fact]
        public async Task UpdateCropAsync_ShouldReturnOk_WhenCropIsUpdated()
        {
            // Arrange
            var expectedError = Error.SetSuccess();
            var crop = new Crop { CropId = 1, CropName = "Corn" };
            var cropId = 1;
            _mockCropUpdatableService.Setup(service => service.UpdateCropAsync(It.IsAny<Crop>()))
                                     .ReturnsAsync(expectedError);

            // Act
            var result = await _controller.UpdateCropAsync(crop, cropId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Null(apiResponse.Error);
            Assert.Equal(SuccessMessage.Added, apiResponse.Data);
        }

        [Fact]
        public async Task UpdateCropAsync_ShouldReturnNotFound_WhenCropDoesNotExist()
        {
            // Arrange
            var expectedError = Error.SetError(ErrorMessage.NotFound, 404);
            var crop = new Crop { CropId = 1, CropName = "Corn" };
            var cropId = 1;
            _mockCropUpdatableService.Setup(service => service.UpdateCropAsync(It.IsAny<Crop>()))
                                     .ReturnsAsync(expectedError);

            // Act
            var result = await _controller.UpdateCropAsync(crop, cropId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(objectResult.Value);
            Assert.Null(apiResponse.Data);
            Assert.Equal(ErrorMessage.NotFound, apiResponse.Error?.Message);
            Assert.Equal(404, apiResponse.Error?.Code);
        }

        [Fact]
        public async Task UpdateCropAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            var crop = new Crop { CropId = 1, CropName = "Corn" };
            var cropId = 1;
            _mockCropUpdatableService.Setup(repo => repo.UpdateCropAsync(It.IsAny<Crop>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdateCropAsync(crop, cropId);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }
    }
}
